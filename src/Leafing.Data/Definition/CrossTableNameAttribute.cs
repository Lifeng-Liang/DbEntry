using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CrossTableNameAttribute : Attribute
    {
        public string Name;

        public CrossTableNameAttribute()
        {
        }

        public CrossTableNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
