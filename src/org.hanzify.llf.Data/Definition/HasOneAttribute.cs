using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class HasOneAttribute : OrderByAttribute
    {
    }
}
