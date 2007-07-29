
#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class OrderByAttribute : Attribute
    {
        public string OrderBy = "";

        public OrderByAttribute()
        {
        }

        public OrderByAttribute(string OrderByString)
        {
            this.OrderBy = OrderByString;
        }
    }
}
