using System;

namespace Lephone.Web.Rails
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DefaultActionAttribute : Attribute
    {
    }
}
