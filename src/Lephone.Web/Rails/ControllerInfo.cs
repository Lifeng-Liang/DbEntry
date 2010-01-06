using System;
using System.Reflection;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class ControllerInfo
    {
        public ControllerInfo(Type t)
        {
            foreach (MethodInfo mi in t.GetMethods(ClassHelper.InstancePublic))
            {
                if (ClassHelper.HasAttribute<DefaultActionAttribute>(mi, false))
                {
                    if (DefaultAction == null)
                    {
                        DefaultAction = mi.Name;
                    }
                    else
                    {
                        throw new WebException("Controller only support one 'DefaultAction', the class is : " + t.Name);
                    }
                }
            }

            if (DefaultAction == null)
            {
                DefaultAction = "list";
            }

            IsScaffolding = ClassHelper.HasAttribute<ScaffoldingAttribute>(t, true);

            ListStyle = ListStyle.Default;
            var lsa = ClassHelper.GetAttribute<ListStyleAttribute>(t, false);
            if(lsa != null)
            {
                ListStyle = lsa.Style;
            }

            ControllerName = t.Name;
            if (ControllerName.EndsWith("Controller"))
            {
                ControllerName = ControllerName.Substring(0, ControllerName.Length - 10);
            }

            Type = t;
        }

        public readonly string DefaultAction;

        public readonly bool IsScaffolding;

        public readonly ListStyle ListStyle;

        public readonly string ControllerName;

        public readonly Type Type;
    }
}
