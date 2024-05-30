using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim {
    public class Configuration {
        public bool WritePolicy { get; set; }
        public bool RowSize { get; set; }
        public bool RowNumber { get; set; }
        public bool Associativity { get; set; }
        public bool ReplacementPolicy { get; set; }
        public bool HitTimespan { get; set; }

        /// <summary>
        /// MemoriaPrincipal
        public bool MPReadTimespan { get; set; }
        /// </summary>
        /// MemoriaPrincipal
        /// </summary>
        public bool MPWriteTimespan { get; set; }
    }
}
