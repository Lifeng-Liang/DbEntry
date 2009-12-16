using System;

namespace Lephone.Web.Rails
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public string ViewName;

        public ViewAttribute(string viewName)
        {
            this.ViewName = viewName;
        }
    }
}
