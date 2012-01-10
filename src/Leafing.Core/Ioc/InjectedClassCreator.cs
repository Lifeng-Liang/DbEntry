using System;
using System.Collections.Generic;
using System.Reflection;

namespace Leafing.Core.Ioc
{
    internal class InjectedClassCreator : ClassCreator
    {
        public readonly ConstructorInfo Constructor;
        public readonly List<PropertyInjector> PropertyInjectors;

        public InjectedClassCreator(Type type, int index, ConstructorInfo constructor, List<PropertyInjector> injectors)
            : base(type, index)
        {
            this.Constructor = constructor;
            this.PropertyInjectors = injectors;
        }

        public override object Create()
        {
            var obj = CreateObject();
            InjectObject(obj);
            return obj;
        }

        private object CreateObject()
        {
            var ps = Constructor.GetParameters();
            var os = new object[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                var injection = ps[i].GetAttribute<InjectionAttribute>(false);
                var op = SimpleContainer.Get(ps[i].ParameterType, injection.Index);
                os[i] = op;
            }
            return Constructor.Invoke(os);
        }

        private void InjectObject(object obj)
        {
            foreach(var injector in PropertyInjectors)
            {
                var op = SimpleContainer.Get(injector.Type, injector.Index);
                injector.Property.SetValue(obj, op, null);
            }
        }
    }
}
