using System.Reflection;
using Leafing.Core;

namespace Leafing.Web.Mvc.Core
{
    public class ActionInfo
    {
        public readonly MethodInfo Method;
        public readonly string Name;
        public readonly string LowerName;
        public readonly string ViewName;

        public ActionInfo(MethodInfo method)
        {
            this.Method = method;
            this.Name = method.Name;
            this.LowerName = this.Name.ToLower();
            this.ViewName = GetViewName();
        }

        private string GetViewName()
        {
            var va = Method.GetAttribute<ViewAttribute>(false);
            return (va == null) ? Method.Name : va.ViewName;
        }
    }
}
