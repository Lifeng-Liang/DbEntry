using System;
using System.Collections.Generic;
using System.Reflection;

namespace Leafing.Web.Mvc.Core
{
    internal static class ControllerFinder
    {
        public static readonly Dictionary<string, ControllerInfo> Controllers;
        public static readonly char[] Spliter = new[] { '/' };
        public static readonly Type CbType = typeof(ControllerBase);

        static ControllerFinder()
        {
            Controllers = new Dictionary<string, ControllerInfo>();
            Controllers["default"] = new ControllerInfo(typeof(DefaultController));
            if (WebSettings.ControllerAssembly == "")
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string s = a.FullName.Split(',')[0];
					if (!s.StartsWith("System.") && !s.StartsWith("Mono.") 
						&& !s.StartsWith("MonoDevelop.") && CouldBeControllerAssemebly(s))
                    {
                        SearchControllers(a);
                    }
                }
            }
            else
            {
                var assembly = Assembly.Load(WebSettings.ControllerAssembly);
                SearchControllers(assembly);
            }
        }

        public static void SearchControllers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsInterface && !type.IsAbstract && type.IsSubclassOf(CbType))
                {
                    var ci = new ControllerInfo(type);
                    Controllers[ci.LowerName] = ci;
                }
            }
        }

        public static bool CouldBeControllerAssemebly(string s)
        {
            switch(s)
            {
                case "Leafing.Data":
                case "Leafing.Core":
                case "Leafing.Web":
                case "mscorlib":
                case "System":
                    return false;
                default:
                    return true;
            }
        }
    }
}
