
#region usings

using System;

#endregion

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AllowNullAttribute : Attribute
    {
    }
}
