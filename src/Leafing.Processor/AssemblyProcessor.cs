using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Leafing.Core.Logging;
using Leafing.Data;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Leafing.Processor
{
    public class AssemblyProcessor
    {
        private static ModuleDefinition _module;
        private readonly string _oldName;
        private readonly string _name;
        private readonly string _sn;
        private readonly string[] _refFiles;
		private readonly SymbolHandler _symbolHandler;

        public static ModuleDefinition Module
        {
            get
            {
                return _module;
            }
        }

        public AssemblyProcessor(string name, string sn, string[] refFiles)
        {
            var n = name.Substring(0, name.Length - 4);
			_symbolHandler = new SymbolHandler(n);
            this._oldName = n + ".bak";
            this._name = name;
            this._sn = sn;
            this._refFiles = refFiles;
			Logger.Default.Info(_symbolHandler.HasSymbols);
        }

        public void Process()
        {
            if (File.Exists(_oldName))
            {
                File.Delete(_oldName);
            }
            _module = ModuleDefinition.ReadModule(_name);

            if (_module.DontNeedToDoAnything())
            {
                Console.WriteLine("Don't need to do anything.");
                return;
            }

            if (_module.IsAssemblyProcessed())
            {
                Console.WriteLine("Already processed!");
                return;
            }

            File.Move(_name, _oldName);

            InitRefFiles();

            Program.Stage = "Process Model";
            ProcessAssembly();
            Program.Stage = "GenerateHandler for Model";
            GenerateHandlers();
        }

        private void InitRefFiles()
        {
            if(_refFiles != null)
            {
                var list = new List<string>();
                foreach (var file in _refFiles)
                {
                    var p = Path.GetDirectoryName(file);
                    if (!list.Contains(p))
                    {
                        list.Add(p);
                        ((DefaultAssemblyResolver)GlobalAssemblyResolver.Instance).AddSearchDirectory(p);
                    }
                    if (!file.EndsWith("Leafing.Core.dll") && !file.EndsWith("Leafing.Data.dll"))
                    {
                        Assembly.LoadFrom(file);
                    }
                }
            }
        }

        private ModuleDefinition ReadModule()
        {
			if(_symbolHandler.HasSymbols)
            {
                return ModuleDefinition.ReadModule(
                    _oldName, new ReaderParameters
                    {
                        ReadSymbols = true,
						SymbolReaderProvider = _symbolHandler.Reader
                    });
            }
            return ModuleDefinition.ReadModule(_oldName);
        }

        private void ProcessAssembly()
        {
            var module = ReadModule();

            var models = GetAllModels(module);

            if (models.Count <= 0)
            {
                Console.WriteLine("Can not find any model to process !");
                return;
            }

            var handler = new KnownTypesHandler(module);

            foreach (var type in models)
            {
                Program.ModelClass = type.FullName;
                var processor = new ModelProcessor(type, handler);
                processor.Process();
            }

            WriteAssembly(module, _oldName);
        }

        private void GenerateHandlers()
        {
            var module = ReadModule();

            var assembly = Assembly.LoadFrom(_oldName);
            var models = DbEntry.GetAllModels(assembly);

            if (models.Count <= 0)
            {
                Console.WriteLine("Can not find any model to generate handler !");
            }

            var handler = new KnownTypesHandler(module);

            foreach (var type in models)
            {
                Program.ModelClass = type.FullName;
                if(Program.ModelClass != null)
                {
                    var model = module.GetType(Program.ModelClass.Replace('+', '/'));
                    var generator = new ModelHandlerGenerator(type, model, handler);
                    var mh = generator.Generate();
                    module.Types.Add(mh);

                    var mhg = new MemberHandlerGenerator(type, model, handler);
                    mhg.Generate(module);

                    new ModelRelationFixer(type, model).Process();
                }
            }

            module.CustomAttributes.Add(handler.GetAssemblyProcessed());

            WriteAssembly(module, _name);
        }



        private void WriteAssembly(ModuleDefinition module, string name)
        {
			var args = _symbolHandler.HasSymbols ? new WriterParameters
                           {
                               WriteSymbols = true,
								SymbolWriterProvider = _symbolHandler.Writer,
                           }
                           : new WriterParameters();
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
            if(t.IsInterface)
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
