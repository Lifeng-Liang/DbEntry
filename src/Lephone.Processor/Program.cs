using System;
using System.Reflection;
using Lephone.Data;
using Mono.Cecil;

namespace Lephone.Processor
{
    public class Program
    {
        //public static List<Type> GetAllModels(Assembly assembly)
        //{
        //    var ts = new List<Type>();
        //    foreach (var t in assembly.GetExportedTypes())
        //    {
        //        if (!t.IsGenericType)
        //        {
        //            foreach (var face in t.GetInterfaces())
        //            {
        //                if(face.FullName == "Lephone.Data.Definition.IDbObject")
        //                {
        //                    ts.Add(t);
        //                }
        //            }
        //        }
        //    }
        //    ts.Sort((x, y) => x.FullName.CompareTo(y.FullName));
        //    return ts;
        //}

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Lephone.Processor ModelAssemblyName");
                return;
            }
            var name = args[0];
            var assembly =  Assembly.LoadFrom(name);
            var models = DbEntry.GetAllModels(assembly);

            var module = ModuleDefinition.ReadModule(name);
            foreach (var model in models)
            {
                var type = module.GetType(model.FullName);
                Console.WriteLine(type.FullName);
            }

            Console.ReadLine();
        }
    }
}
