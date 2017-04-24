using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Leafing.Core.Setting;
using Leafing.Core.Logging;

namespace Leafing.Core.Ioc
{
    public static class SimpleContainer
    {
        private static readonly Dictionary<Type, ImplementsCreators> Container
            = new Dictionary<Type, ImplementsCreators>();

        static SimpleContainer()
        {
            if (!IocConfig.EnableAutoLoad) { return; }
            var assemblies = (IocConfig.Assemblies.Count == 0) ? GetAllAssemblies() : IocConfig.Assemblies;
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
            var pathBin = GetIocSearchPath(path, IocConfig.SearchPath);
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
                var main = Assembly.GetEntryAssembly();
                if(main != null)
                {
                    assemblies.Add(main);
                }
                foreach (var fn in Directory.GetFiles(path))
                {
                    var file = fn.ToLower();
                    if (file.EndsWith(".dll"))
                    {
						try {
							var assembly = Assembly.LoadFrom(fn);
							assemblies.Add(assembly);
						} catch (Exception ex) {
							Logger.System.Warn(ex);
						}
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
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
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        var entry = type.GetAttribute<DependenceEntryAttribute>(false);
                        if (entry != null)
                        {
                            entries.Add(type);
                        }
                        var impl = type.GetAttribute<ImplementationAttribute>(false);
                        if (impl != null)
                        {
                            impls.Add(ClassCreator.New(type, impl.Index, impl.Name));
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Loading assembly error: " + assembly.FullName + ex);
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
