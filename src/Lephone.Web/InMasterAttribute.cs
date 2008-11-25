using System;

namespace Lephone.Web
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InMasterAttribute : Attribute
    {
    }
}
