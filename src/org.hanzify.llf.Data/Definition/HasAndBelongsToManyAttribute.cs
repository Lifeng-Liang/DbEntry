
#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HasAndBelongsToManyAttribute : OrderByAttribute
    {
    }
}
