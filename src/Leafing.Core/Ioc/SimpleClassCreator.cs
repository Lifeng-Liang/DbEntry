using System;
using System.Reflection;

namespace Leafing.Core.Ioc
{
    internal class SimpleClassCreator : ClassCreator
    {
        public readonly Func<object> Constructor;

        public SimpleClassCreator(Type type, int index, string name, ConstructorInfo constructor) 
            : base(type, index, name)
        {
            Constructor = ClassHelper.GetConstructorDelegate(constructor);
        }

        public override object Create()
        {
            return Constructor();
        }
    }
}
