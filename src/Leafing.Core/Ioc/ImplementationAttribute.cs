using System;

namespace Leafing.Core.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationAttribute : Attribute
    {
        public readonly int Index;

        public ImplementationAttribute(int index)
        {
            this.Index = index;
        }
    }
}
