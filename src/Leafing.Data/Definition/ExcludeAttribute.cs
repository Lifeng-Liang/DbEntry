using System;

namespace Leafing.Data.Definition {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeAttribute : Attribute {
    }
}