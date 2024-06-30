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

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─────────────────────────────┐");
            Console.WriteLine("│ Impacto do Tamanho da Cache │");
            Console.WriteLine("└─────────────────────────────┘");
            Console.BackgroundColor = ConsoleColor.Black;
            //Impacto_Tamanho_Cache
            foreach (int Test in new int[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }) {
                Impacto_Tamanho_Cache.QuantidadeBloco = Test;
                Result result = new MemoryTest(Impacto_Tamanho_Cache, lstAddresses).Result;
                result.Log();
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─────────────────────────────┐");
            Console.WriteLine("│ Impacto do Tamanho do Bloco │");
            Console.WriteLine("└─────────────────────────────┘");
            Console.BackgroundColor = ConsoleColor.Black;
            //Impacto_Tamanho_Bloco
            List<Tuple<int, int>> lstTesteTamanhoBloco = new(){
                new (4   , 2048),
                new (8   , 1024),
                new (16  , 512),
                new (32  , 256),
                new (64  , 128),
                new (128 , 64),
                new (256 , 32),
                new (512 , 16),
                new (1024, 8),
                new (2048, 4),
            };
            foreach (var item in lstTesteTamanhoBloco) {
                Impacto_Tamanho_Bloco.TamanhoBloco    = item.Item1;
                Impacto_Tamanho_Bloco.QuantidadeBloco = item.Item2;
                Result result = new MemoryTest(Impacto_Tamanho_Bloco, lstAddresses).Result;
                result.Log();
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌────────────────────────────┐");
            Console.WriteLine("│ Impacto da Associatividade │");
            Console.WriteLine("└────────────────────────────┘");
            Console.BackgroundColor = ConsoleColor.Black;
            //Impacto_Associatividade
            foreach (int Test in new int[] { 1, 2, 4, 8, 16, 32, 64 }) {
                Impacto_Associatividade.BlocosPorConjunto = Test;
                Result result = new MemoryTest(Impacto_Associatividade, lstAddresses).Result;
                result.Log();
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─────────────────────────────────────┐");
            Console.WriteLine("│ Impacto da Política de Substituição │");
            Console.WriteLine("└─────────────────────────────────────┘");
            Console.BackgroundColor = ConsoleColor.Black;
            //Impacto Politica Substituicao
            foreach (ReplacementPolicy Test in new ReplacementPolicy[] { ReplacementPolicy.LRU, ReplacementPolicy.LFU, ReplacementPolicy.Random }) {
                Impacto_Politica_Substituicao.ReplacementPolicy = Test;
                Result result = new MemoryTest(Impacto_Politica_Substituicao, lstAddresses).Result;
                result.Log();
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─────────────────────────────┐");
            Console.WriteLine("│ Largura de Banda da Memória │");
            Console.WriteLine("└─────────────────────────────┘");
            Console.BackgroundColor = ConsoleColor.Black;
            //Impacto_Banda_Memoria [ Review ]

            foreach (WritePolicy WritePolicy in new WritePolicy[] { WritePolicy.WriteBack, WritePolicy.WriteTrough }) {
                Impacto_Banda_Memoria.WritePolicy = WritePolicy;
                foreach (var TamCache in new Tuple<int, int>[] { new(128,64), new(64,128) }) {
                    Impacto_Banda_Memoria.TamanhoBloco = TamCache.Item1;   
                    Impacto_Banda_Memoria.QuantidadeBloco = TamCache.Item2;   
                    foreach (int Associatividade in new int[] { 2, 4 }) {
                        Impacto_Banda_Memoria.BlocosPorConjunto = Associatividade;
                        Result result = new MemoryTest(Impacto_Banda_Memoria, lstAddresses).Result;
                        result.Log();
                    }
                }
            }
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
            QuantidadeBloco = 64,
            //[ Varia ]
            BlocosPorConjunto = 4,
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
