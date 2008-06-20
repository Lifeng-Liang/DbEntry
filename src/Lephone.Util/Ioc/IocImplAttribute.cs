using System;

namespace Lephone.Util.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IocImplAttribute : Attribute
    {
        public string Name { get; set; }

        public IocImplAttribute()
            : this(SimpleContainer.DefaultName)
        {
        }

        public IocImplAttribute(string name)
        {
            this.Name = name;
        }
    }
}
