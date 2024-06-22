using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim
{
    public class MemoryTest {

        /// <summary>
        /// in ns
        /// </summary>
        public int HitTime { get; set; } = 10;
        public int WriteTime { get; set; } = 60;

        public int Hits { get; set; } = 0;
        public int Misses { get; set; } = 0;

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


        private void CalculateBits(Configuration configuration) {
            //[ Em bytes ]
            TamanhoCache     = configuration.QuantidadeBloco * configuration.TamanhoBloco;
            
            QuantidadeLinhas    = TamanhoCache / configuration.Associativity;
            QuantidadeConjuntos = QuantidadeLinhas / configuration.Associativity;
            LinhaSize        = (int)Math.Log2(QuantidadeConjuntos);
            PalavraSize      = (int)Math.Log2(configuration.TamanhoBloco);
            RotuloSize = TotalAddressSize - PalavraSize - LinhaSize;
        }

        public MemoryTest(Configuration configuration, List<Address> lstAdresses) {
            CalculateBits(configuration);
            SetMemory();

            foreach (Address adress in lstAdresses) {
                TestAdress(adress);
            }

            decimal HitRate          = (100 * Hits) / (Hits + Misses);
            decimal AverageAcessTime = (Misses * configuration.MPReadTimespan) + Hits * this.HitTime;

        }

        public void TestAdress(Address adress) {
            bool Hit = false;

            string Binary = Convert.ToString(Convert.ToInt64(adress.Path, 16), 2).PadLeft(TotalAddressSize, '0');
            string Path = Binary.Substring(0, RotuloSize);
            if (Memory.Any(p => p != null && p.Path == Path)) {
                Hit = true;    
            }
            else {
                Hit = false;

                int index = new Random().Next(0, Memory.Length - 1);
                MemoryAddress? mem = this.Memory.ElementAt(index);
                if (mem == null) {
                    this.Memory[index] = new MemoryAddress(Path);
                } else {
                    mem.Path = Path;
                }
            }


            if (Hit) { this.Hits++;   }
            else     { this.Misses++; }
        }

        public void SetMemory() {
            Memory = new MemoryAddress[this.QuantidadeLinhas];
        }

        /// <summary>
        /// T1 - Rotulo
        /// T2 - Conteudo 
        /// </summary>
        public MemoryAddress[] Memory;
    }

    public class MemoryAddress {
        public string Path { get; set; } = "";

        public MemoryAddress(string Path) {
            this.Path = Path;
        }
    }
}
