using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using Lephone.Data.Definition;
using Lephone.Core;

namespace Lephone.Web.Mvc
{
    public class PageBase : Page
    {
        protected internal string ControllerName;
        protected internal string ActionName;
        internal Dictionary<string, object> Bag = new Dictionary<string, object>();

        protected internal LinkToInfo LinkTo
        {
            get { return new LinkToInfo(ControllerName); }
        }

        protected internal UrlToInfo UrlTo
        {
            get { return new UrlToInfo(ControllerName); }
        }

        protected object this[string key]
        {
            get
            {
                return Bag[key];
            }
            set
            {
                Bag[key] = value;
            }
        }

        public FlashHandler Flash = new FlashHandler();

        public new SessionHandler Session = new SessionHandler();

        protected override void OnInit(System.EventArgs e)
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
                if (!ClassHelper.HasAttribute<ExcludeAttribute>(info, false))
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


