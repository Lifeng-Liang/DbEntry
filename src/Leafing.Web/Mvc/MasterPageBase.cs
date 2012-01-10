using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI;
using Leafing.Core;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    public class MasterPageBase : MasterPage
    {
        private Dictionary<string, object> _bag = new Dictionary<string, object>();

        internal Dictionary<string, object> Bag
        {
            get { return _bag; }
            set
            {
                _bag = value;
                if(Master != null && Master is MasterPageBase)
                {
                    ((MasterPageBase)Master).Bag = _bag;
                }
            }
        }

        protected object this[string key]
        {
            get { return Bag[key]; }
            set { Bag[key] = value; }
        }

        protected internal string ControllerName;
        protected internal string ActionName;

        public FlashHandler Flash = new FlashHandler();

        protected internal LinkToInfo LinkTo(string controllerName = null)
        {
            return LinkHelper.LinkTo(controllerName ?? ControllerName);
        }

        protected internal LinkToInfo LinkTo<T>(Expression<Action<T>> expr = null) where T : ControllerBase
        {
            return LinkHelper.LinkTo(expr);
        }

        protected internal UrlToInfo UrlTo(string controllerName = null)
        {
            return LinkHelper.UrlTo(controllerName ?? ControllerName);
        }

        protected internal UrlToInfo UrlTo<T>(Expression<Action<T>> expr = null) where T : ControllerBase
        {
            return LinkHelper.UrlTo(expr);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Master != null && Master is MasterPageBase)
            {
                ((MasterPageBase)Master).Bag = Bag;
                ((MasterPageBase)Master).InitFields();
            }
        }

        internal void InitFields()
        {
            var infos = GetType().GetFields(ClassHelper.InstancePublic | BindingFlags.DeclaredOnly);
            foreach (FieldInfo info in infos)
            {
                if (!info.HasAttribute<ExcludeAttribute>(false))
                {
                    object value = this[info.Name];
                    info.SetValue(this, value);
                }
            }
            if (Master != null && Master is MasterPageBase)
            {
                ((MasterPageBase)Master).InitFields();
            }
        }

        protected internal void Print(object o)
        {
            Response.Write(o);
        }

        protected internal void Print(string s)
        {
            Response.Write(s);
        }
    }
}


