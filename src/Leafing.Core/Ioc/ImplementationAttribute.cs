using System;

namespace Leafing.Core.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationAttribute : Attribute
    {
        internal readonly int Index;
        internal readonly string Name;

        public ImplementationAttribute(int index)
        {
            this.Index = index;
        }

        public ImplementationAttribute(string name)
        {
            this.Name = name;
        }

        public ImplementationAttribute(int index, string name)
        {
            this.Index = index;
            this.Name = name;
        }
    }
}
