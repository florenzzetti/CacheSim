using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim
{
    public class MemoryTest {
        Configuration configuration;

        public int Hits { get; set; } = 0;
        public int Misses { get; set; } = 0;
        public int WritesMP { get; set; } = 0;
        public int ReadsMP { get; set; } = 0;

        public readonly int TotalAddressSize = 32;
        
        /// <summary>
        /// bits
        /// </summary>
        private int RotuloSize;
        /// <summary>
        /// bits
        /// </summary>
        private int LinhaSize;
        /// <summary>
        /// bits
        /// </summary>
        private int PalavraSize;

        private int TamanhoCache;
        private int QuantidadeLinhas { get; set; }
        private int QuantidadeConjuntos { get; set; }



        private void CalculateBits() {
            //[ Em bytes ]
            TamanhoCache        = configuration.QuantidadeBloco * configuration.TamanhoBloco;
            
            QuantidadeLinhas    = TamanhoCache / configuration.TamanhoBloco;
            QuantidadeConjuntos = QuantidadeLinhas / configuration.BlocosPorConjunto;
            LinhaSize        = (int)Math.Log2(QuantidadeConjuntos);
            PalavraSize      = (int)Math.Log2(configuration.TamanhoBloco);
            RotuloSize       = TotalAddressSize - PalavraSize - LinhaSize;
        }

        public MemoryTest(Configuration configuration, List<Address> lstAdresses) {
            this.configuration = configuration;
            CalculateBits();
            SetMemory();

            foreach (Address adress in lstAdresses) { 
                TestAdress(adress);
            }
            if(configuration.WritePolicy == WritePolicy.WriteBack) {
                this.WritesMP += this.Memory!.Sum(p => p.lstAddresses.Where(p => p != null).Count(z => z.Dirty));
            }

            this.HitRate          = (100 * Hits) / (Hits + Misses);
        }

        int AverageAcessTime { get; set; }
        decimal HitRate { get; set; }

        public Result Result { get { 
            return new Result(
                configuration, 
                AverageAcessTime, 
                HitRate, 
                WritesMP, 
                ReadsMP,
                QuantidadeLinhas,
                QuantidadeConjuntos,
                LinhaSize,
                PalavraSize,
                RotuloSize
            );    
        } }

        public void TestAdress(Address address) {
            string Binary  = Convert.ToString(Convert.ToInt64(address.Path, 16), 2).PadLeft(TotalAddressSize, '0');
            string TagPath      = Binary.Substring(0         , RotuloSize);
            string ConjuntoPath = Binary.Substring(RotuloSize, LinhaSize);
            
            MemoryAddress?  inCacheConj  = this.Memory.FirstOrDefault(p => p != null && p.ConjuntoPath == ConjuntoPath);
            MemoryAddressTag? inCacheTag = inCacheConj != null ? inCacheConj.lstAddresses.FirstOrDefault(p => p!= null && p.TagPath == TagPath) : null;

            if (address.ReadWrite == ReadWrite.Read) {
                if (inCacheTag != null) {
                    this.Hits++;

                    foreach (MemoryAddress memoryAddress in Memory) {
                        foreach (MemoryAddressTag? memoryAddressTag in memoryAddress.lstAddresses) {
                            if(memoryAddressTag != null) {
                                memoryAddressTag.Idade++;
                            }
                        }
                    }
                    inCacheTag.Reads++;
                    inCacheTag.Idade = 0;
                }
                else {
                    foreach (MemoryAddress memoryAddress in Memory) {
                        foreach (MemoryAddressTag? memoryAddressTag in memoryAddress.lstAddresses) {
                            if (memoryAddressTag != null) {
                                memoryAddressTag.Idade++;
                            }
                        }
                    }
                    this.Misses++;
                    this.ReadsMP++;
                    WriteInCache(ConjuntoPath, TagPath, ReadWrite.Read);
                }
            }
            // [ Write ] 
            else {
                // [ Review Hit ] 
                if (inCacheTag != null) {
                    inCacheTag.Dirty = true; 
                    Hits++; 
                }
                else {
                    if(configuration.WritePolicy == WritePolicy.WriteTrough) {
                        Misses++;
                        this.WritesMP++;
                    }else {
                        Misses++;
                        ReadsMP++;
                        WriteInCache(ConjuntoPath, TagPath, ReadWrite.Write, true);
                    }
                }
            }
        }

        private void WriteInCache(string ConjuntoPath, string TagPath, ReadWrite ReadWrite, bool Dirty = false) {
            MemoryAddress? inCacheConj = this.Memory.FirstOrDefault(p => p != null && p.ConjuntoPath == ConjuntoPath);
            if(inCacheConj == null) {
                int index = this.Memory.IndexOf(this.Memory.First(p => p.ConjuntoPath == null));
                Memory[index] = GetNewMemoryAddress(ConjuntoPath);
                inCacheConj = Memory[index];
            } 
            
            

            int ReplacementIndex = -1;
            if(inCacheConj.lstAddresses.All(p => p != null)) {
                if (configuration.ReplacementPolicy == ReplacementPolicy.LFU) {
                    ReplacementIndex = GetLFU(inCacheConj);
                }
                else
                if (configuration.ReplacementPolicy == ReplacementPolicy.LRU) {
                    ReplacementIndex = GetLRU(inCacheConj);
                }
                else
                if (configuration.ReplacementPolicy == ReplacementPolicy.Random) {
                    ReplacementIndex = new Random().Next(0, inCacheConj.lstAddresses.Capacity - 1);
                }
            }
            else {
                ReplacementIndex = inCacheConj.lstAddresses.IndexOf(inCacheConj.lstAddresses.First(p => p == null));
            }

            MemoryAddressTag CurrentMem = inCacheConj.lstAddresses[ReplacementIndex]!;
            if (configuration.WritePolicy == WritePolicy.WriteBack) {
                if (CurrentMem != null && CurrentMem.Dirty) { WritesMP++; }
            }
            
            inCacheConj.lstAddresses[ReplacementIndex] = new MemoryAddressTag(TagPath, Dirty: ReadWrite == ReadWrite.Write ? true : false, Reads: ReadWrite == ReadWrite.Read ? 1 : 0);
            return;
        }

        private int GetLRU(MemoryAddress inCacheConj) {
            MemoryAddressTag? AddressToBeReplaced = inCacheConj.lstAddresses.FirstOrDefault(p => p != null && p.Idade == inCacheConj.lstAddresses.Where(z => z != null).Max(x => x?.Idade));
            if(AddressToBeReplaced == null) { return 0; }
            return inCacheConj.lstAddresses.IndexOf(AddressToBeReplaced);
        }

        private int GetLFU(MemoryAddress inCacheConj) {
            MemoryAddressTag? AddressToBeReplaced = inCacheConj.lstAddresses.FirstOrDefault(p => p != null && p.Reads == inCacheConj.lstAddresses.Where(z => z != null).Min(x => x?.Reads));
            if (AddressToBeReplaced == null) { return 0; }
            return inCacheConj.lstAddresses.IndexOf(AddressToBeReplaced);
        }

        public void SetMemory() { 
            Memory = new();
            Memory.Capacity = this.QuantidadeConjuntos;
            for(int i = 0; i < QuantidadeConjuntos; i++) {
                MemoryAddress address = GetNewMemoryAddress(null);

                Memory.Add(address);
            }
        }
        public List<MemoryAddress> Memory;

        private MemoryAddress GetNewMemoryAddress(string? ConjuntoPath) {
            MemoryAddress address = new MemoryAddress(ConjuntoPath);
            for (int j = 0; j < configuration.BlocosPorConjunto; j++) {
                address.lstAddresses.Add(null);
            }
            return address;
        }
    }

    public class MemoryAddress {
        public string? ConjuntoPath { get; set; }
        public List<MemoryAddressTag?> lstAddresses { get; set; } = new();
        public MemoryAddress(string? ConjuntoPath) {
            this.ConjuntoPath = ConjuntoPath;
        }
    }

    public class MemoryAddressTag {
        public int Reads { get; set; } = 0;
        public int Idade { get; set; } = 0;
        public string TagPath { get; set; }
        public bool Dirty { get; set; }
        public MemoryAddressTag(string TagPath, bool Dirty = false, int Reads = 0, int Idade = 0) {
            this.TagPath = TagPath;
            this.Dirty = Dirty;
            this.Reads = Reads;
            this.Idade = Idade;
        }
    }

    public class Result {
        Configuration configuration;
        public int AverageAcessTime { get; set; }
        public decimal HitRate { get; set; }
        public int WritesMP { get; set; } = 0;
        public int ReadsMP { get; set; } = 0;

        public int QuantidadeLinhas { get; set; }
        public int QuantidadeConjuntos { get; set; }
        public int LinhaSize { get; set; }
        public int PalavraSize { get; set; }
        public int RotuloSize { get; set; }

        public Result(
            Configuration configuration, 
            int AverageAcessTime, 
            decimal HitRate, 
            int WritesMP, 
            int ReadsMP,
            int QuantidadeLinhas,
            int QuantidadeConjuntos,
            int LinhaSize,
            int PalavraSize,
            int RotuloSize
        ) {
            this.configuration = configuration;
            this.AverageAcessTime = AverageAcessTime;
            this.HitRate = HitRate;
            this.WritesMP = WritesMP;
            this.ReadsMP = ReadsMP;

            this.QuantidadeLinhas = QuantidadeLinhas;
            this.QuantidadeConjuntos = QuantidadeConjuntos;
            this.LinhaSize = LinhaSize;
            this.PalavraSize = PalavraSize;
            this.RotuloSize  = RotuloSize;

        }

        public void Log() {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[ Testando Configuração! ]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"    - Tamanho do Bloco:          {configuration.TamanhoBloco}b");
            Console.WriteLine($"    - Quantidade de Blocos:      {configuration.QuantidadeBloco}");
            Console.WriteLine($"    - Blocos por Conjunto:       {configuration.BlocosPorConjunto}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"        - Tamanho da Cache:          {configuration.QuantidadeBloco * configuration.TamanhoBloco}b");
            Console.WriteLine($"        - Quantidade de Linhas:      {QuantidadeLinhas}");
            Console.WriteLine($"        - Quantidade de Conjuntos:   {QuantidadeConjuntos}");
            Console.WriteLine($"        - Rotulo(Bits):              {RotuloSize}");
            Console.WriteLine($"        - Linha(Bits):               {LinhaSize}");
            Console.WriteLine($"        - Palavra(Bits):             {PalavraSize}");
            Console.ForegroundColor = ConsoleColor.White;
            string WPolicy = configuration.WritePolicy == WritePolicy.WriteTrough ? "Write Trough" : "Write Back";
            Console.WriteLine($"    - Política de escrita:       {WPolicy}");
            string RPolicy = configuration.ReplacementPolicy == ReplacementPolicy.LRU ? "Least Recently Used [LRU]" :
                             configuration.ReplacementPolicy == ReplacementPolicy.LFU ? "Least Frenquently Used [LFU]" : "Random";
            Console.WriteLine($"    - Algoritmo de Substituição: {RPolicy}");
            Console.WriteLine($"    - Tempo de acerto:           {configuration.HitTimespan}ns");
            Console.WriteLine($"    - Tempo de leitura na MP:    {configuration.MPReadTimespan}ns");
            Console.WriteLine($"    - Tempo de escrita na MP:    {configuration.MPWriteTimespan}ns");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"    - [ Resultados: ]");
            Console.WriteLine($"        - Taxa de Acerto:        {(this.HitRate).ToString("n4")}%");
            Console.WriteLine($"        - Tempo médio de acesso: {(configuration.HitTimespan + (1 - (HitRate/100)) * configuration.MPReadTimespan).ToString("n4")}ns");
            Console.WriteLine($"        - Leituras na MP:        {ReadsMP}");
            Console.WriteLine($"        - Escritas na MP:        {WritesMP}");
        }
    }
}
