
#region usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Lephone.Data.Definition
{
    public abstract class OrderByAttribute : Attribute
    {
        public string OrderBy;
    }
}
