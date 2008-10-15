using System;

namespace Lephone.Web
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PageLoadAttribute : Attribute
    {
    }
}
