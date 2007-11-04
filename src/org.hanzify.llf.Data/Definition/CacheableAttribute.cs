
using System;
using System.Collections.Generic;
using System.Text;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheableAttribute : Attribute
    {
    }
}
