using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Util.Setting;

namespace Lephone.Util.IoC
{
    public static class SimpleContainer
    {
        private static readonly Dictionary<Type, Dictionary<string, Type>> Container
            = new Dictionary<Type, Dictionary<string, Type>>();

        internal static readonly string DefaultName = "";

        static SimpleContainer()
        {
            if(UtilSetting.IOC_EnableAutoLoad)
            {
                var entries = new List<Type>();
                var impls = new List<KeyValuePair<string, Type>>();
                SearchAssemblies(entries, impls);
                foreach (var entry in entries)
                {
                    var list = impls.FindAll(t => entry.Equals(t.Value) || ClassHelper.IsChildOf(entry, t.Value));
                    foreach (var pair in list)
                    {
                        Register(entry, pair.Value, pair.Key, true);
                    }
                }
            }
        }

        private static void SearchAssemblies(ICollection<Type> entries, ICollection<KeyValuePair<string, Type>> impls)
        {
            for (int i = 1; ; i++)
            {
                var s = string.Format("IOC.SearchAssembly.{0}", i);
                var asmName = ConfigHelper.DefaultSettings.GetValue(s);
                if(string.IsNullOrEmpty(asmName))
                {
                    break;
                }
                var asm = Assembly.Load(asmName);
                foreach (var type in asm.GetTypes())
                {
                    var entry =ClassHelper.GetAttribute<DependenceEntryAttribute>(type, false);
                    if(entry != null)
                    {
                        entries.Add(type);
                    }
                    var impl = ClassHelper.GetAttribute<ImplementationAttribute>(type, false);
                    if(impl != null)
                    {
                        impls.Add(new KeyValuePair<string, Type>(impl.Name, type));
                    }
                }
            }
        }

        public static void Register<TI, TO>() where TO : TI
        {
            Register<TI, TO>(DefaultName);
        }

        public static void Register<TI, TO>(string name) where TO : TI
        {
            Register<TI, TO>(name, true);
        }
        public static void Register<TI, TO>(string name, bool throwException) where TO : TI
        {
            Register(typeof(TI), typeof(TO), name, throwException);
        }

        public static void Register(Type ti, Type to, string name, bool throwException)
        {
            if(to.IsInterface || to.IsAbstract)
            {
                throw new ArgumentException("Impl type could not be interface or abstract class", "to");
            }
            if(Container.ContainsKey(ti))
            {
                var value = Container[ti];
                if(value.ContainsKey(name))
                {
                    if(throwException)
                    {
                        throw new UtilException("Duplicated Register.");
                    }
                }
                lock (value)
                {
                    value[name] = to;
                }
            }
            else
            {
                lock (Container)
                {
                    Container[ti] = new Dictionary<string, Type> { { name, to } };
                }
            }
        }

        public static T Get<T>()
        {
            return Get<T>(DefaultName);
        }

        public static T Get<T>(string name)
        {
            return (T) Get(typeof (T), name);
        }

        public static object Get(Type t, string name)
        {
            if (Container.ContainsKey(t))
            {
                var value = Container[t];
                if (value.ContainsKey(name))
                {
                    object obj = CreateInjectableObject(value[name]);
                    InjectProperties(obj);
                    return obj;
                }
            }
            throw new UtilException("Can not found {0} in SimpleContainer", name);
        }

        private static object CreateInjectableObject(Type type)
        {
            var cis = type.GetConstructors(ClassHelper.InstancePublic);
            if(cis.Length == 0 || cis.Length > 1)
            {
                throw new UtilException("IoC object must have only one public constractor.");
            }
            var ci = cis[0];
            var ps = ci.GetParameters();
            var os = new object[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                var ia = ClassHelper.GetAttribute<InjectionAttribute>(ps[i], false);
                string name = ia == null ? DefaultName : ia.Name;
                object op = Get(ps[i].ParameterType, name);
                os[i] = op;
            }
            return ci.Invoke(os);
        }

        private static void InjectProperties(object obj)
        {
            Type t = obj.GetType();
            var ps = t.GetProperties(ClassHelper.AllFlag);
            foreach (var p in ps)
            {
                var ia = ClassHelper.GetAttribute<InjectionAttribute>(p, false);
                if(ia != null)
                {
                    object op = Get(p.PropertyType, ia.Name);
                    p.SetValue(obj, op, null);
                }
            }
        }
    }
}
