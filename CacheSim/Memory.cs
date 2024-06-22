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

        private int RotuloSize;
        private int LinhaSize;
        private int PalavraSize;

        public int Hits { get; set; } = 0;
        public int Misses { get; set; } = 0;

        public MemoryTest(Configuration configuration, List<Address> lstAdresses) {
            foreach (Address adress in lstAdresses) {
                TestAdress(adress);
            }

            decimal HitRate          = (100 * Hits) / (Hits + Misses);
            decimal AverageAcessTime = (Misses * configuration.MPReadTimespan) + Hits * this.HitTime;

        }

        public void TestAdress(Address adress) {
            //TO-DO [ Test ]

            bool Hit = false;
            if (Hit) { this.Hits++;   }
            else     { this.Misses++; }
        }

        /// <summary>
        /// T1 - Rotulo
        /// T2 - Conteudo 
        /// </summary>
        public Tuple<int, object> Memory;
    
    }
}
