using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheableAttribute : Attribute
    {
    }
}
