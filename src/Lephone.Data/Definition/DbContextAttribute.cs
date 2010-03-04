using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbContextAttribute : Attribute
    {
        public string ContextName;

        public DbContextAttribute(string contextName)
        {
            this.ContextName = contextName;
        }
    }
}
