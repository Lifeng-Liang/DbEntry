using System;

namespace Lephone.Util.IoC
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
