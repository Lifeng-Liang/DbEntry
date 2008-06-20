using System;

namespace Lephone.Util.Ioc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class IocEntryAttribute : Attribute
    {
    }
}
