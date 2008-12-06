using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lephone.Util;

namespace Lephone.CodeGen
{
    class Program
    {
        static void ShowHelp()
        {
            var s = ResourceHelper.ReadToEnd(typeof (Program), "Readme.txt");
            Console.WriteLine(s);
        }

        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2 || args[0].ToLower() != "a")
                {
                    ShowHelp();
                    return 1;
                }

                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("The file you input doesn't exist!");
                    ShowHelp();
                    return 2;
                }

                if (args.Length == 2)
                {
                    SearchClasses(args[1]);
                    return 0;
                }

                GenerateAspNetTemplate(args[1], args[2]);

                return 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 999;
        }

        private static void GenerateAspNetTemplate(string fileName, string className)
        {
            EnumTypes(fileName, t =>
                                {
                                    if(t.FullName == className)
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
            EnumTypes(fileName, t =>
                                {
                                    Console.WriteLine(t.FullName);
                                    return true;
                                });
        }

        private static void EnumTypes(string fileName, CallbackHandler<Type, bool> callback)
        {
            Assembly dll = Assembly.LoadFile(fileName);
            Type idot = Type.GetType("Lephone.Data.Definition.IDbObject, Lephone.Data", true);
            var ts = new List<Type>();
            foreach (Type t in dll.GetExportedTypes())
            {
                var lt = new List<Type>(t.GetInterfaces());
                if (lt.Contains(idot))
                {
                    ts.Add(t);
                }
            }
            ts.Sort(new TypeComparer());
            foreach (Type t in ts)
            {
                if(!callback(t))
                {
                    break;
                }
            }
        }
    }
}
