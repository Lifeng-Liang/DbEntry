
#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    public abstract class OrderByAttribute : Attribute
    {
        public string OrderBy;
    }
}
