
using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SoftDeleteAttribute : Attribute
    {
        public string ColumnName = "IsDeleted";
    }
}
