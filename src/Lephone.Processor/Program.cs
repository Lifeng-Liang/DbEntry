using System;
using Mono.Cecil;

namespace Lephone.Processor
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Lephone.AssemblyProcessor ModelAssemblyName");
                return;
            }
            var name = args[0];
            var module = ModuleDefinition.ReadModule(name);

            foreach (var type in module.Types)
            {
                if (type.IsDbModel())
                {
                    Console.WriteLine(type.FullName);
                }
            }

            module.Write(name);
        }
    }
}
