using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class PageBase : Page
    {
        protected internal string ControllerName;
        protected internal string ActionName;
        internal Dictionary<string, object> Bag = new Dictionary<string, object>();
        private LinkToInfo.LinkTo _linkTo;
        private UrlToInfo.UrlTo _urlTo;

        protected internal LinkToInfo.LinkTo LinkTo
        {
            get { return _linkTo ?? (_linkTo = new LinkToInfo.LinkTo(ControllerName)); }
        }

        protected internal UrlToInfo.UrlTo UrlTo
        {
            get { return _urlTo ?? (_urlTo = new UrlToInfo.UrlTo(ControllerName)); }
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
                    object value = this[info.Name];
                    info.SetValue(this, value);
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
