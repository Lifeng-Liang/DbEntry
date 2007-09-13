
using System;
using System.Collections.Generic;
using System.Text;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class SpecialNameAttribute : Attribute
    {
    }
}
