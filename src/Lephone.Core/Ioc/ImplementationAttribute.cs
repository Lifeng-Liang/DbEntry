using System;

namespace Lephone.Core.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationAttribute : Attribute
    {
        public string Name { get; set; }

        public ImplementationAttribute()
            : this(SimpleContainer.DefaultName)
        {
        }

        public ImplementationAttribute(string name)
        {
            this.Name = name;
        }
    }
}
