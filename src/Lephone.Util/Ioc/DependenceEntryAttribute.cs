using System;

namespace Lephone.Util.IoC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DependenceEntryAttribute : Attribute
    {
    }
}
