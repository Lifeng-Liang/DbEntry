using System;

namespace Leafing.Data.Definition {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SpecialNameAttribute : Attribute {
    }
}
