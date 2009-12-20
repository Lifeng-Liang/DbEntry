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
                    if (_defaultAction == null)
                    {
                        _defaultAction = mi.Name;
                    }
                    else
                    {
                        throw new WebException("Controller only support one 'DefaultAction', the class is : " + t.Name);
                    }
                }
            }
            _isScaffolding = ClassHelper.HasAttribute<ScaffoldingAttribute>(t, true);
            _listStyle = ListStyle.Default;
            var lsa = ClassHelper.GetAttribute<ListStyleAttribute>(t, false);
            if(lsa != null)
            {
                _listStyle = lsa.Style;
            }
        }

        private ControllerInfo() {}

        private string _defaultAction;

        public string DefaultAction
        {
            get { return _defaultAction ?? "list"; }
        }

        private bool _isScaffolding;

        public bool IsScaffolding
        {
            get { return _isScaffolding; }
        }

        private ListStyle _listStyle;

        public ListStyle ListStyle
        {
            get { return _listStyle; }
        }
    }
}
