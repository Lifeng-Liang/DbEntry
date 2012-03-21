using System;
using System.IO;
using Leafing.Data;

namespace Leafing.Processor
{
    class Program
    {
        public static string Stage = "NONE";
        public static string ModelClass = "<NULL>";

        static int Main(string[] args)
        {
            try
            {
                ModelContext.LoadHandler = false;
                Process(args);
                return 0;
            }
            catch (ArgsErrorException ex)
            {
                if (ex.ReturnCode != 0)
                {
                    Console.WriteLine(ex);
                }
                ShowHelp();
                return ex.ReturnCode;
            }
            catch (ModelException ex)
            {
                Console.WriteLine("{0} : {1}", Stage, ModelClass);
                Console.WriteLine(ex);
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
    Leafing.Processor ModelsFileName [KeyFileName]

ModelsFileName: The assembly file name who has the model classes.
KeyFileName:    The strong name key pair file name.

Example:
    Leafing.Processor Models.dll [@ReferenceFile1;ReferenceFile2...]
    Leafing.Processor Models.dll models.snk [@ReferenceFile1;ReferenceFile2...]
");
        }

        private static void Process(string[] args)
        {
            if(args.Length == 0)
            {
                throw new ArgsErrorException(1, "Input Error!");
            }

            var fileName = Path.GetFullPath(args[0]);

            if (!File.Exists(fileName))
            {
                throw new ArgsErrorException(2, "The file you input doesn't exist!");
            }

            if (args.Length == 1 || args.Length == 2 || args.Length == 3)
            {
                var sn = args.Length == 1 ? null : (args[1].StartsWith("@") ? null : Path.GetFullPath(args[1]));

                if (!sn.IsNullOrEmpty() && !File.Exists(sn))
                {
                    throw new ArgsErrorException(3, "The sn file you input doesn't exist!");
                }

                new AssemblyProcessor(fileName, sn, GetReferenceFiles(args[args.Length - 1])).Process();
                Console.WriteLine("Assembly processed!");
            }
        }

        private static string[] GetReferenceFiles(string files)
        {
            if(files.StartsWith("@"))
            {
                files = files.Substring(1);
                var fs = files.Split(';');
                return fs;
            }
            return null;
        }
    }
}
