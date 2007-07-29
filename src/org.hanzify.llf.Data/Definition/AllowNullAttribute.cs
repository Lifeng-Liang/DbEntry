
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AllowNullAttribute : Attribute
    {
    }
}
