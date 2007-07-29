
#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BelongsToAttribute : Attribute
    {
    }
}
