using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CrossTableNameAttribute : Attribute
    {
        public string Name;

        public CrossTableNameAttribute()
        {
        }

        public CrossTableNameAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
