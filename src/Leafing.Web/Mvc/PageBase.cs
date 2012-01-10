using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.UI;
using Leafing.Core;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    public class PageBase : Page
    {
        protected internal string ControllerName;
        protected internal string ActionName;
        internal Dictionary<string, object> Bag = new Dictionary<string, object>();

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

        protected object this[string key]
        {
            get { return Bag[key]; }
            set { Bag[key] = value; }
        }

        public FlashHandler Flash = new FlashHandler();

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
            var infoList = GetType().GetFields(ClassHelper.InstancePublic | BindingFlags.DeclaredOnly);
            foreach (FieldInfo info in infoList)
            {
                if (!info.HasAttribute<ExcludeAttribute>(false))
                {
                    if(Bag.ContainsKey(info.Name))
                    {
                        object value = this[info.Name];
                        info.SetValue(this, value);
                    }
                    else
                    {
                        throw new WebException("Can not find [{0}] in bag.", info.Name);
                    }
                }
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


