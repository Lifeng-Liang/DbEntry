using System;
using System.IO;
using Lephone.Util;

namespace Lephone.CodeGen
{
    internal class Program
    {
        private static int Main(string[] args)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 999;
        }

        private static bool ActionMatch(string s)
        {
            if (s == null)
            {
                return false;
            }
            s = s.ToLower();
            return (s == "a" || s == "ra" || s == "rv");
        }

        private static void Process(string[] args)
        {
            if (args.Length < 2 || !ActionMatch(args[0]))
            {
                throw new ArgsErrorException(0, null);
            }

            if (!File.Exists(args[1]))
            {
                throw new ArgsErrorException(2, "The file you input doesn't exist!");
            }

            if (args.Length == 2)
            {
                SearchClasses(args[1]);
                return;
            }

            switch (args[0].ToLower())
            {
                case "a":
                    GenerateAspNetTemplate(args[1], args[2]);
                    break;
                case "ra":
                    if (args.Length >= 4)
                    {
                        var gen = new RailsActionGenerator(args[1], args[2], args[3]);
                        string s = gen.ToString();
                        Console.WriteLine(s);
                    }
                    else
                    {
                        throw new ArgsErrorException(3, "Need class name and action name.");
                    }
                    break;
                case "rv":
                    if (args.Length >= 4)
                    {
                        var gen = new RailsViewGenerator(args[1], args[2], args[3]);
                        string s = gen.ToString();
                        Console.WriteLine(s);
                    }
                    else
                    {
                        throw new ArgsErrorException(4, "Need class name and view name.");
                    }
                    break;
            }
        }

        private static void GenerateAspNetTemplate(string fileName, string className)
        {
            Helper.EnumTypes(fileName, t =>
            {
                if (t.FullName == className)
                {
                    var tb = new AspNetGenerator(t);
                    Console.WriteLine(tb.ToString());
                    return false;
                }
                return true;
            });
        }

        private static void SearchClasses(string fileName)
        {
            Helper.EnumTypes(fileName, t =>
            {
                Console.WriteLine(t.FullName);
                return true;
            });
        }

        private static void ShowHelp()
        {
            string s = ResourceHelper.ReadToEnd(typeof(Program), "Readme.txt");
            Console.WriteLine(s);
        }
    }
}
