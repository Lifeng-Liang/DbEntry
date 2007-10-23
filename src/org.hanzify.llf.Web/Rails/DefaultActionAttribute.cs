
using System;

namespace Lephone.Web.Rails
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultActionAttribute : Attribute
    {
        public string ActionName;

        public DefaultActionAttribute(string ActionName)
        {
            this.ActionName = ActionName;
        }
    }
}
