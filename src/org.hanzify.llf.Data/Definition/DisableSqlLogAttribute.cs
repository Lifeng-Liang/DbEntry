
using System;

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class DisableSqlLogAttribute : Attribute
    {
        public DisableSqlLogAttribute() {}
    }
}
