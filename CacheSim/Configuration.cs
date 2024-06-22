using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim {
    public class Configuration {
        public WritePolicy WritePolicy { get; set; }
        public int RowSize { get; set; }
        public int RowNumber { get; set; }
        public int Associativity { get; set; }
        public ReplacementPolicy ReplacementPolicy { get; set; }
        public int HitTimespan { get; set; }

        /// <summary>
        /// MemoriaPrincipal
        public int MPReadTimespan { get; set; }
        /// </summary>
        /// MemoriaPrincipal
        /// </summary>
        public int MPWriteTimespan { get; set; }
    }

    public enum WritePolicy {
        WriteTrough = 0,
        WriteBack   = 1,
    }
    public enum ReplacementPolicy {
        LFU    = 0,
        LRU    = 1,
        Random = 2,
    }

    public enum ReadWrite {
        Read  = 0,
        Write = 1
    }

    public class Address {
        public string Path { get; set; }
        public ReadWrite ReadWrite { get; set; }

        public Address(string Path, ReadWrite ReadWrite) {
            this.Path = Path;
            this.ReadWrite = ReadWrite;
        }
    }
}
