
using System;
using System.Collections.Generic;
using System.Text;

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LazyLoadAttribute : Attribute
    {
    }
}
