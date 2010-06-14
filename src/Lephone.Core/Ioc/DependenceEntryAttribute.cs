using System;

namespace Lephone.Core.Ioc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DependenceEntryAttribute : Attribute
    {
    }
}
