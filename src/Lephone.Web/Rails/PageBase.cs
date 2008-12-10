using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class PageBase : Page
    {
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();
        protected internal string ControllerName;
        protected internal string ActionName;

        public FlashBox Flash = new FlashBox();

        public new SessionBox Session = new SessionBox();

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            if (Master != null && Master is MasterPageBase)
            {
                ((MasterPageBase)Master).bag = bag;
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
                    object value = bag[info.Name];
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

        protected internal string LinkTo(LTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Controller))
            {
                args.Controller = ControllerName;
            }
            return MasterPageBase.LinkTo(args, paramters);
        }

        protected internal string UrlTo(UTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Controller))
            {
                args.Controller = ControllerName;
            }
            return MasterPageBase.UrlTo(args.Controller, args.Action, paramters);
        }

    }
}
