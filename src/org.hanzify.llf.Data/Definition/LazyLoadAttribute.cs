
using System;
using System.Collections.Generic;
using System.Text;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LazyLoadAttribute : Attribute
    {
    }
}
