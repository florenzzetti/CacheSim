using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheSim
{
    public static class Tests {
        public static int DefaultHitTimeSpan = 10;
        public static int DefaultMPTimespan = 60;

        public static void Run(List<Address> lstAddresses) {
            //Impacto_Tamanho_Cache
            foreach (int Test in new int[] { 16, 32, 64, 128, 256, 512 }) {
                Impacto_Tamanho_Cache.QuantidadeBloco = Test;
                Result result = new MemoryTest(Impacto_Tamanho_Cache, lstAddresses).Result;
                result.Log();
            }

            ////Impacto_Tamanho_Bloco
            //List<Tuple<int, int>> lstTesteTamanhoBloco = new(){
            //    new (4   , 2048),
            //    new (8   , 1024),
            //    new (16  , 512),
            //    new (32  , 256),
            //    new (64  , 128),
            //    new (128 , 64),
            //    new (256 , 32),
            //    new (512 , 16),
            //    new (1024, 8),
            //    new (2048, 4),
            //};
            //foreach (var item in lstTesteTamanhoBloco) {
            //    Impacto_Tamanho_Bloco.TamanhoBloco    = item.Item1;
            //    Impacto_Tamanho_Bloco.QuantidadeBloco = item.Item2;

            //    Result result = new MemoryTest(Impacto_Tamanho_Bloco, lstAddresses).Result;
            //    result.Log();
            //}

            ////Impacto_Associatividade
            //foreach (int Test in new int[1, 2, 4, 8, 16, 32, 64]) {
            //    Impacto_Associatividade.BlocosPorConjunto = Test;
            //    Result result = new MemoryTest(Impacto_Associatividade, lstAddresses).Result;
            //    result.Log();
            //}

            ////Impacto Politica Substituicao [ Review ]
            //foreach (ReplacementPolicy Test in new ReplacementPolicy[0, 1, 2]) {
            //    Impacto_Politica_Substituicao.ReplacementPolicy = Test;
            //    Result result = new MemoryTest(Impacto_Politica_Substituicao, lstAddresses).Result;
            //    result.Log();
            //}


            //Impacto_Banda_Memoria [ Review ]
            //foreach (WritePolicy WritePolicy in new WritePolicy[0, 1]) {
            //    foreach (var TamCache in new Tuple<int, int>[] { new(128,64), new(64,128) }) {

            //    }
            //}

                //Impacto_Politica_Substituicao.ReplacementPolicy = (ReplacementPolicy)Test;
                //Result result = new MemoryTest(Impacto_Politica_Substituicao, lstAddresses).Result;
                //result.Log();
        }

        public static Configuration Impacto_Tamanho_Cache =
        new Configuration
        {
            TamanhoBloco = 128,
            WritePolicy = WritePolicy.WriteTrough,
            BlocosPorConjunto = 4,
            HitTimespan = DefaultHitTimeSpan,
            MPReadTimespan = DefaultMPTimespan,
            MPWriteTimespan = DefaultMPTimespan,
            ReplacementPolicy = ReplacementPolicy.LRU,
            //[ Varia ]
            QuantidadeBloco = 16,
        };

        public static Configuration Impacto_Tamanho_Bloco= 
        new Configuration {
            WritePolicy  = WritePolicy.WriteTrough,
            HitTimespan = DefaultHitTimeSpan,
            MPReadTimespan  = DefaultMPTimespan,
            MPWriteTimespan = DefaultMPTimespan,
            ReplacementPolicy = ReplacementPolicy.LRU,
            BlocosPorConjunto = 2,
            //[ ??? ]

            //[ Varia ]
            QuantidadeBloco = 0,
            TamanhoBloco    = 0,
        };

        public static Configuration Impacto_Associatividade = 
        new Configuration {
            TamanhoBloco = 128,
            WritePolicy  = WritePolicy.WriteBack,
            HitTimespan = DefaultHitTimeSpan,
            MPReadTimespan  = DefaultMPTimespan,
            MPWriteTimespan = DefaultMPTimespan,
            ReplacementPolicy = ReplacementPolicy.LRU,
            //[ Varia ]
            BlocosPorConjunto = 4,
            QuantidadeBloco = 16,
        };

        public static Configuration Impacto_Politica_Substituicao = 
        new Configuration {
            TamanhoBloco = 128,
            WritePolicy  = WritePolicy.WriteTrough,
            HitTimespan = DefaultHitTimeSpan,
            MPReadTimespan  = DefaultMPTimespan,
            MPWriteTimespan = DefaultMPTimespan,
            BlocosPorConjunto = 4,
            //[ Varia ]
            ReplacementPolicy = ReplacementPolicy.LFU,
            QuantidadeBloco = 16,
        };

        public static Configuration Impacto_Banda_Memoria = 
        new Configuration {
            HitTimespan = DefaultHitTimeSpan,
            MPReadTimespan  = DefaultMPTimespan,
            MPWriteTimespan = DefaultMPTimespan,
            ReplacementPolicy = ReplacementPolicy.LRU,
            //[ Varia ]
            BlocosPorConjunto = 4,
            TamanhoBloco = 128,
            WritePolicy  = WritePolicy.WriteTrough,
            QuantidadeBloco = 16,
        };
    }

    public class Test {
        public Configuration First { get; set; }
    }
}
