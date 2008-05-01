
using System;
using System.Reflection;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    internal class ControllerInfo : FlyweightBase<Type, ControllerInfo>
    {
        protected override void Init(Type t)
        {
            foreach (MethodInfo mi in t.GetMethods(ClassHelper.InstancePublic))
            {
                if (ClassHelper.HasAttribute<DefaultActionAttribute>(mi, false))
                {
                    if (_DefaultAction == null)
                    {
                        _DefaultAction = mi.Name;
                    }
                    else
                    {
                        throw new WebException("Controller only support one 'DefaultAction', the class is : " + t.Name);
                    }
                }
            }
            if (ClassHelper.HasAttribute<ScaffoldingAttribute>(t, true))
            {
                _IsScaffolding = true;
            }
        }

        private ControllerInfo() {}

        private string _DefaultAction;

        public string DefaultAction
        {
            get { return _DefaultAction ?? "list"; }
        }

        private bool _IsScaffolding;

        public bool IsScaffolding
        {
            get { return _IsScaffolding; }
        }
    }
}
