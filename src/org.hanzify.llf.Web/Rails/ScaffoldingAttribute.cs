
using System;
using System.Collections.Generic;
using System.Text;

namespace Lephone.Web.Rails
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple= false, Inherited=true)]
    internal class ScaffoldingAttribute : Attribute
    {
    }
}
