using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim
{
    public class MemoryTest {
        Configuration configuration;

        /// <summary>
        /// in ns
        /// </summary>
        private int HitTime { get; set; } = 10;


        public int Hits { get; set; } = 0;
        public int Misses { get; set; } = 0;
        public int WritesMP { get; set; } = 0;

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
            
            QuantidadeLinhas    = TamanhoCache / configuration.BlocosPorConjunto;
            QuantidadeConjuntos = QuantidadeLinhas / configuration.BlocosPorConjunto;
            LinhaSize        = (int)Math.Log2(QuantidadeConjuntos);
            PalavraSize      = (int)Math.Log2(configuration.TamanhoBloco);
            RotuloSize       = TotalAddressSize - PalavraSize - LinhaSize;
        }

        public MemoryTest(Configuration configuration, List<Address> lstAdresses) {
            this.configuration = configuration;
            CalculateBits();
            SetMemory();

            foreach (Address adress in lstAdresses) { TestAdress(adress); }
            WritesMP += Memory!.Count(p => p != null && p.Dirty);


            decimal HitRate          = (100 * Hits) / (Hits + Misses);
            decimal AverageAcessTime = (Misses * configuration.MPReadTimespan) + Hits * this.HitTime;
        }

        public Result Result { get { 
            return new Result(configuration);    
        } }

        public void TestAdress(Address address) {
            bool Hit = false;

            string Binary  = Convert.ToString(Convert.ToInt64(address.Path, 16), 2).PadLeft(TotalAddressSize, '0');
            string TagPath = Binary.Substring(0, RotuloSize);
            bool inCache   = Memory.Any(p => p != null && p.TagPath == TagPath);
            MemoryAddress? inCacheMem = this.Memory.FirstOrDefault(p => p.TagPath == TagPath);

            if (address.ReadWrite == ReadWrite.Read) {
                if (inCache) { 
                    Hit = true;
                    inCacheMem!.Reads++;
                    RecentlyUsed.Remove(inCacheMem);
                    RecentlyUsed.Insert(0, inCacheMem);
                }
                else {
                    Hit = false;
                    MemoryAddress NewAdress = new MemoryAddress(TagPath, Reads: 1);
                    this.Memory.Add(NewAdress);
                    RecentlyUsed.Insert(0, NewAdress);
                }
            }
            // address.ReadWrite == ReadWrite.Write
            else {
                //[ Review Hit ] 
                if (inCache) {
                    inCacheMem!.Dirty = true;
                }

                if(!inCache && configuration.WritePolicy == WritePolicy.WriteTrough) {
                    this.WritesMP++;
                }else
                if (!inCache && Memory.All(p => p != null)) {
                    int ReplacementIndex = -1;
                    if(configuration.ReplacementPolicy == ReplacementPolicy.LFU) {
                        ReplacementIndex = Memory.IndexOf(Memory.First(p => p !=null && p.Reads == Memory.Where(z => z != null).Min(x => x?.Reads)));
                    }else
                    if (configuration.ReplacementPolicy == ReplacementPolicy.LRU) {
                        ReplacementIndex = Memory.IndexOf(RecentlyUsed.Last());
                    }else
                    if (configuration.ReplacementPolicy == ReplacementPolicy.Random) {
                        ReplacementIndex = new Random().Next(0, Memory.Capacity - 1);
                    }

                    MemoryAddress CurrentMem = Memory[ReplacementIndex]!;
                    if(configuration.WritePolicy == WritePolicy.WriteBack && CurrentMem.Dirty) {
                        WritesMP++;
                    }
                    Memory[ReplacementIndex] = new MemoryAddress(TagPath);
                }else
                //configuration.ReplacementPolicy == WritePolicy.WriteBack
                if (!inCache) {
                    Memory.Add(new MemoryAddress(TagPath, true));
                }
            }


            if (Hit) { this.Hits++;   }
            else     { this.Misses++; }
        }

        public void SetMemory() { 
            Memory = new List<MemoryAddress?>();
            Memory.Capacity = QuantidadeLinhas;
            for(int i = 0; i < QuantidadeLinhas; i++) {
                Memory[i] = null;
            }
        }
        public List<MemoryAddress?> Memory;

        //[ MostrecentlyUsed = first ]
        public List<MemoryAddress> RecentlyUsed = new();
    }

    public class MemoryAddress {
        public string TagPath { get; set; }
        public bool Dirty { get; set; }
        public int Reads { get; set; }
        public MemoryAddress(string TagPath, bool Dirty = false, int Reads = 0) {
            this.TagPath = TagPath;
            this.Dirty   = Dirty;
            this.Reads   = Reads; 
        }
    }

    public class Result {
        Configuration configuration;
        public Result(Configuration configuration) {
            this.configuration = configuration;
        }

        public void Log() {
            Console.WriteLine("[ Logging ]");
        }
    }
}
