using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim {
    public class Output {
        Configuration Configuration { get; set; }
        public int TotalAdresses { get; set; }
        public int Writes { get; set; }
        public int Reads { get; set; }
        public decimal HitRate { get; set; }

        /// <summary>
        /// In ns
        /// </summary>
        public decimal AverageCacheTimespan { get; set; }
    }
}