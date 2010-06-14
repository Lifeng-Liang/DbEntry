using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Core;
using Mono.Cecil;

namespace Lephone.CodeGen.Processor
{
    public class AssemblyProcessor
    {
        private static readonly Dictionary<Type, MethodInfo> TypeDict;

        static AssemblyProcessor()
        {
            TypeDict = new Dictionary<Type, MethodInfo>();
            var types = new[] { typeof(Date), typeof(Time), typeof(DateTime), typeof(Guid), typeof(TimeSpan), typeof(decimal), typeof(string) };
            foreach (var type in types)
            {
                var mi = type.GetMethod("op_Inequality", ClassHelper.AllFlag);
                TypeDict.Add(type, mi);
            }
        }

        public void Process(string name)
        {
            var module = ModuleDefinition.ReadModule(name);

            if(module.IsAssemblyProcessed())
            {
                Console.WriteLine("Already processed!");
                return;
            }

            var models = GetAllModels(module);

            if(models.Count <= 0)
            {
                Console.WriteLine("Can not find any model!");
                return;
            }

            var handler = new KnownTypesHandler(module);
            foreach (var model in models)
            {
                Console.WriteLine(model.FullName);
                var processor = new ModelProcessor(model, handler);
                processor.Process();
            }
            foreach (var model in models)
            {
                var generator = new ModelHandlerGenerator(model, handler);
                var mh = generator.Generate();
                module.Types.Add(mh);
            }

            module.CustomAttributes.Add(handler.GetAssemblyProcessed());

            module.Write(name);
        }

        public static List<TypeDefinition> GetAllModels(ModuleDefinition assembly)
        {
            var ts = new List<TypeDefinition>();
            foreach (var t in assembly.Types)
            {
                if (!t.IsGenericInstance && !t.IsAbstract)
                {
                    if(IsModel(t))
                    {
                        ts.Add(t);
                    }
                }
            }
            ts.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            return ts;
        }

        private static bool IsModel(TypeDefinition t)
        {
            if(t.Name.StartsWith("<"))
            {
                return false;
            }
            foreach (var @interface in t.Interfaces)
            {
                if (@interface.FullName == KnownTypesHandler.DbObjectInterface)
                {
                    return true;
                }
            }
            if(t.FullName == KnownTypesHandler.DbObjectSmartUpdate)
            {
                return true;
            }
            if(t.FullName == KnownTypesHandler.Object)
            {
                return false;
            }
            return IsModel(t.BaseType.Resolve());
        }
    }
}
