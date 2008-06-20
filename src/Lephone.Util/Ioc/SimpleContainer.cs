using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Util.Setting;

namespace Lephone.Util.Ioc
{
    public static class SimpleContainer
    {
        private static readonly Dictionary<Type, Dictionary<string, Type>> container
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
                    var list = impls.FindAll(t => entry.Equals(t.Value) || ClassHelper.IsChildrenOf(entry, t.Value));
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
                    var entry =ClassHelper.GetAttribute<IocEntryAttribute>(type, false);
                    if(entry != null)
                    {
                        entries.Add(type);
                    }
                    var impl = ClassHelper.GetAttribute<IocImplAttribute>(type, false);
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
            if(to.IsInterface)
            {
                throw new ArgumentException("Impl type could not be interface", "to");
            }
            if(container.ContainsKey(ti))
            {
                var value = container[ti];
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
                lock (container)
                {
                    container[ti] = new Dictionary<string, Type> { { name, to } };
                }
            }
        }

        public static T Get<T>()
        {
            return Get<T>(DefaultName);
        }

        public static T Get<T>(string name)
        {
            var t = typeof (T);
            if(container.ContainsKey(t))
            {
                var value = container[t];
                if (value.ContainsKey(name))
                {
                    return (T)ClassHelper.CreateInstance(value[name]);
                }
            }
            throw new UtilException("Can not found {0} in SimpleContainer", name);
        }
    }
}