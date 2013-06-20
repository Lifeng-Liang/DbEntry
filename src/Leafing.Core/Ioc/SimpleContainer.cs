using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Leafing.Core.Setting;

namespace Leafing.Core.Ioc
{
    public static class SimpleContainer
    {
        private static readonly Dictionary<Type, ImplementsCreators> Container
            = new Dictionary<Type, ImplementsCreators>();

        static SimpleContainer()
        {
            if (!CoreSettings.IocEnableAutoLoad) { return; }
            var assemblies = GetAllAssemblies();
            InitCustomAssemblies(assemblies);
            SearchInAssemblies(assemblies);
        }

        private static List<Assembly> GetAllAssemblies()
        {
            var path = SystemHelper.BaseDirectory;
            if(path == null)
            {
                throw new SystemException("Can not find BaseDirectory!!!!");
            }
            var list = new List<Assembly>();
            SearchAssemblies(list, path);
            var pathBin = GetIocSearchPath(path, CoreSettings.IocSearchPath);
            if(!pathBin.IsNullOrEmpty())
            {
                SearchAssemblies(list, pathBin);
            }
            return list;
        }

        private static string GetIocSearchPath(string path, string binPath)
        {
            var pathBin = Path.Combine(path, binPath);
            if(Directory.Exists(pathBin))
            {
                return pathBin;
            }
            var p = binPath.ToLower();
            if (p != binPath)
            {
                return GetIocSearchPath(path, p);
            }
            return null;
        }

        private static void SearchAssemblies(List<Assembly> assemblies, string path)
        {
            try
            {
                foreach (var fn in Directory.GetFiles(path))
                {
                    var file = fn.ToLower();
                    if ((file.EndsWith(".dll") || file.EndsWith(".exe")) && !file.EndsWith(".vshost.exe"))
                    {
                        var assembly = Assembly.LoadFrom(fn);
                        assemblies.Add(assembly);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        private static void InitCustomAssemblies(List<Assembly> assemblies)
        {
            var list = new List<Assembly>();
            for (int i = 1; ; i++)
            {
                var s = string.Format("IocAssembly.{0}", i);
                var asmName = ConfigHelper.DefaultSettings.GetValue(s);
                if(string.IsNullOrEmpty(asmName) || asmName.Length <= 1)
                {
                    break;
                }
                var realAsmName = asmName.Substring(1);
                switch(asmName[0])
                {
                    case '+':
                        var asm = Assembly.Load(realAsmName);
                        if(!assemblies.Contains(asm))
                        {
                            list.Add(asm);
                        }
                        break;
                    case '-':
                        foreach (var assembly in assemblies)
                        {
                            if(assembly.FullName == realAsmName)
                            {
                                assemblies.Remove(assembly);
                            }
                        }
                        break;
                    default:
                        throw new IocException("IocAssembly value must start with [+] or [-]");
                }
            }
        }

        private static void SearchInAssemblies(List<Assembly> assemblies)
        {
            var entries = new List<Type>();
            var impls = new List<ClassCreator>();
            Search(assemblies, entries, impls);
            foreach (var entry in entries)
            {
                var list = impls.FindAll(cc => Match(entry, cc.Type));
                foreach (var cc in list)
                {
                    Register(entry, cc);
                }
            }
        }

        private static void Search(List<Assembly> assemblies, List<Type> entries, List<ClassCreator> impls)
        {
            foreach(var assembly in assemblies)
            {
                foreach(var type in assembly.GetTypes())
                {
                    var entry = type.GetAttribute<DependenceEntryAttribute>(false);
                    if(entry != null)
                    {
                        entries.Add(type);
                    }
                    var impl = type.GetAttribute<ImplementationAttribute>(false);
                    if(impl != null)
                    {
                        impls.Add(ClassCreator.New(type, impl.Index, impl.Name));
                    }
                }
            }
        }

        private static bool Match(Type entry, Type impl)
        {
            return entry == impl || impl.IsChildOf(entry);
        }

        public static void Register(Type entryType, Type implType, int index, string name)
        {
            var cc = ClassCreator.New(implType, index, name);
            Register(entryType, cc);
        }

        private static void Register(Type entryType, ClassCreator cc)
        {
            if (Container.ContainsKey(entryType))
            {
                var creator = Container[entryType];
                creator.Add(cc);
            }
            else
            {
                Container[entryType] = new ImplementsCreators(cc);
            }
        }

        public static T Get<T>()
        {
            return Get<T>(0);
        }

        public static T Get<T>(int index)
        {
            return (T)Get(typeof(T), index);
        }

        public static object Get(Type entry, int index)
        {
            if (Container.ContainsKey(entry))
            {
                var creator = Container[entry];
                return creator.Create(index);
            }
            throw new CoreException("Can not find [{0}] index [{1}]", entry, index);
        }

        public static T Get<T>(string name)
        {
            return (T)Get(typeof(T), name);
        }

        public static object Get(Type entry, string name)
        {
            if (Container.ContainsKey(entry))
            {
                var creator = Container[entry];
                return creator.Create(name);
            }
            throw new CoreException("Can not find [{0}] name [{1}]", entry, name);
        }
    }
}
