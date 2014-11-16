using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Leafing.Core.Ioc
{
    public static class IocConfig
    {
        public static bool EnableAutoLoad { get; private set; }
        public static string SearchPath { get; private set; }
        public static List<Assembly> Assemblies { get; private set; }

        static IocConfig ()
	    {
            EnableAutoLoad = true;
            SearchPath = "Bin";
            Assemblies = new List<Assembly>();
	    }

        public static void SetSettings(bool enableAutoLoad, string searchPath = null)
        {
            EnableAutoLoad = enableAutoLoad;
            if(searchPath != null)
            {
                SearchPath = searchPath;
            }
        }

        public static void AddAssembly(Assembly assembly)
        {
            if(assembly == null)
            {
                throw new ArgumentNullException();
            }
            Assemblies.Add(assembly);
        }

        public static void AddAssembly(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName) || assemblyName.Length <= 1)
            {
                throw new ArgumentNullException();
            }
            var assembly = Assembly.Load(assemblyName);
            AddAssembly(assembly);
        }
    }
}
