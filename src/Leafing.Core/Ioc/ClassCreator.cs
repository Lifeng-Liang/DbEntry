using System;
using System.Collections.Generic;

namespace Leafing.Core.Ioc
{
    internal abstract class ClassCreator
    {
        public readonly Type Type;
        public readonly int Index;

        protected ClassCreator(Type type, int index)
        {
            this.Type = type;
            this.Index = index;
        }

        public abstract object Create();

        public static ClassCreator New(Type type, int index)
        {
            if (index <= 0 || index > 100)
            {
                throw new ArgumentOutOfRangeException("index", "index should be between 1 to 100");
            }
            if (type.IsInterface || type.IsAbstract)
            {
                throw new ArgumentException("Implement type could not be interface or abstract class", "type");
            }
            var cis = type.GetConstructors(ClassHelper.InstancePublic);
            if (cis.Length == 0 || cis.Length > 1)
            {
                throw new CoreException("[{0}] Ioc object must have only one public constractor.", type);
            }
            var constructor = cis[0];
            var injectors = GetInjectors(type);
            if(injectors.Count > 0 || constructor.GetParameters().Length > 0)
            {
                return new InjectedClassCreator(type, index, constructor, injectors);
            }
            return new SimpleClassCreator(type, index, constructor);
        }

        private static List<PropertyInjector> GetInjectors(Type type)
        {
            var list = new List<PropertyInjector>();
            var properties = type.GetProperties(ClassHelper.AllFlag);
            foreach (var property in properties)
            {
                var injection = property.GetAttribute<InjectionAttribute>(false);
                if (injection != null)
                {
                    list.Add(new PropertyInjector(property, injection.Index));
                }
            }
            return list;
        }
    }
}
