using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class HasManyAttribute : OrderByAttribute
    {
    }
}
