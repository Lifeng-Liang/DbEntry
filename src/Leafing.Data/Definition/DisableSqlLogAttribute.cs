using System;

namespace Leafing.Data.Definition {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisableSqlLogAttribute : Attribute {
    }
}