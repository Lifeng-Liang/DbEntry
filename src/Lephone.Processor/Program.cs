using System;
using System.IO;
using Lephone.Data;

namespace Lephone.Processor
{
    class Program
    {
        public static string Stage = "NONE";
        public static string ModelClass = "<NULL>";

        static int Main(string[] args)
        {
            try
            {
                Process(args);
                return 0;
            }
            catch (ArgsErrorException ex)
            {
                if (ex.ReturnCode != 0)
                {
                    Console.WriteLine(ex.Message);
                }
                ShowHelp();
                return ex.ReturnCode;
            }
            catch (ModelException ex)
            {
                Console.WriteLine("{0} : {1}", Stage, ModelClass);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} : {1}", Stage, ModelClass);
                Console.WriteLine(ex);
            }
            return 999;
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"
Usage for process the dll which models included:
    Lephone.Processor ModelsFileName [KeyFileName]

ModelsFileName: The assembly file name who has the model classes.
KeyFileName:    The strong name key pair file name.

Example:
    Lephone.Processor Models.dll
    Lephone.Processor Models.dll models.snk
");
        }

        private static void Process(string[] args)
        {
            var fileName = Path.GetFullPath(args[0]);

            if (!File.Exists(fileName))
            {
                throw new ArgsErrorException(2, "The file you input doesn't exist!");
            }

            if (args.Length == 1 || args.Length == 2)
            {
                var sn = args.Length == 1 ? null : Path.GetFullPath(args[1]);

                if (!sn.IsNullOrEmpty() && !File.Exists(sn))
                {
                    throw new ArgsErrorException(3, "The sn file you input doesn't exist!");
                }

                new AssemblyProcessor(fileName, sn).Process();
                Console.WriteLine("Assembly processed!");
                return;
            }
        }

    }
}
