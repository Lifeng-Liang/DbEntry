using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lephone.Data;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Lephone.Processor
{
    public class AssemblyProcessor
    {
        private static ModuleDefinition _module;
        private readonly string _oldName;
        private readonly string _name;
        private readonly string _sn;

        public static ModuleDefinition Module
        {
            get
            {
                return _module;
            }
        }

        public AssemblyProcessor(string name, string sn)
        {
            this._oldName = name.Substring(0, name.Length - 4) + ".bak";
            this._name = name;
            this._sn = sn;
        }

        public void Process()
        {
            if(File.Exists(_oldName))
            {
                File.Delete(_oldName);
            }

            _module = ModuleDefinition.ReadModule(_name);

            if (_module.IsAssemblyProcessed())
            {
                Console.WriteLine("Already processed!");
                return;
            }

            File.Move(_name, _oldName);

            ProcessAssembly();
            Console.WriteLine();
            GenerateModelHandler();
        }

        private void ProcessAssembly()
        {
            var module = ModuleDefinition.ReadModule(
                _oldName, new ReaderParameters
                              {
                                  ReadSymbols = true,
                                  SymbolReaderProvider = ProcesssorSettings.GetSymbolReaderProvider()
                              });

            var models = GetAllModels(module);

            if (models.Count <= 0)
            {
                Console.WriteLine("Can not find any model to process !");
                return;
            }

            var handler = new KnownTypesHandler(module);

            foreach (var type in models)
            {
                Console.WriteLine(type.FullName);
                var processor = new ModelProcessor(type, handler);
                processor.Process();
            }

            WriteAssembly(module, _oldName);
        }

        private void GenerateModelHandler()
        {
            var module = ModuleDefinition.ReadModule(
                _oldName, new ReaderParameters
                              {
                                  ReadSymbols = true,
                                  SymbolReaderProvider = ProcesssorSettings.GetSymbolReaderProvider()
                              });

            var assembly = Assembly.LoadFrom(_oldName);
            var models = DbEntry.GetAllModels(assembly);

            if (models.Count <= 0)
            {
                Console.WriteLine("Can not find any model to generate handler !");
            }

            var handler = new KnownTypesHandler(module);

            foreach (var type in models)
            {
                Console.WriteLine(type.FullName);
                var model = module.GetType(type.FullName.Replace('+', '/'));
                var generator = new ModelHandlerGenerator(type, model, handler);
                var mh = generator.Generate();
                module.Types.Add(mh);
            }

            module.CustomAttributes.Add(handler.GetAssemblyProcessed());

            WriteAssembly(module, _name);
        }

        private void WriteAssembly(ModuleDefinition module, string name)
        {
            var args = new WriterParameters
                           {
                               WriteSymbols = true,
                               SymbolWriterProvider = ProcesssorSettings.GetSymbolWriterProvider(),
                           };
            if (_sn.IsNullOrEmpty())
            {
                module.Write(name, args);
            }
            else
            {
                using(var s = new FileStream(_sn, FileMode.Open, FileAccess.Read))
                {
                    args.StrongNameKeyPair = new StrongNameKeyPair(s);
                    module.Write(name, args);
                }
            }
        }

        public static List<TypeDefinition> GetAllModels(ModuleDefinition assembly)
        {
            var ts = new List<TypeDefinition>();
            CollectModel(ts, assembly.Types);
            ts.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            return ts;
        }

        private static void CollectModel(List<TypeDefinition> ts, Collection<TypeDefinition> types)
        {
            if(types != null)
            {
                foreach (var type in types)
                {
                    if (IsModel(type))
                    {
                        ts.Add(type);
                    }
                    CollectModel(ts, type.NestedTypes);
                }
            }
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
            if (t.FullName == KnownTypesHandler.DbObjectSmartUpdate)
            {
                return true;
            }
            if (t.FullName == KnownTypesHandler.Object || t.BaseType == null || t.BaseType.Namespace.ToLower().StartsWith("nunit."))
            {
                return false;
            }

            var baseType = t.BaseType;
            if (baseType.FullName.StartsWith(KnownTypesHandler.DbObjectModel1))
            {
                return true;
            }
            if (baseType.FullName.StartsWith(KnownTypesHandler.DbObjectModel2))
            {
                return true;
            }
            if (baseType.FullName == KnownTypesHandler.DbObjectSmartUpdate)
            {
                return true;
            }
            return IsModel(baseType.Resolve());
        }
    }
}
