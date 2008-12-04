using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class MasterPageBase : MasterPage
    {
        private Dictionary<string, object> _bag = new Dictionary<string, object>();

        protected internal Dictionary<string, object> bag
        {
            protected get
            {
                return _bag;
            }
            set
            {
                _bag = value;
                if(Master != null && Master is MasterPageBase)
                {
                    ((MasterPageBase)Master).bag = _bag;
                }
            }
        }

        protected internal string ControllerName;
        protected internal string ActionName;

        protected FlashBox Flash = new FlashBox();

        protected new SessionBox Session = new SessionBox();

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
            var infos = GetType().GetFields(ClassHelper.InstancePublic | BindingFlags.DeclaredOnly);
            foreach (FieldInfo info in infos)
            {
                if (!ClassHelper.HasAttribute<ExcludeAttribute>(info, false))
                {
                    object value = bag[info.Name];
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

        protected static internal string LinkTo(LTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Title))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                UrlTo(args.ToUTArgs(), paramters),
                args.Title,
                args.Addon == null ? "" : " " + args.Addon);
            return ret;
        }

        protected internal static string UrlTo(UTArgs args, params object[] paramters)
        {
            return UrlTo(args.Controller, args.Action, paramters);
        }

        public static string UrlTo(string Controller, string Action, params object[] paramters)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath;
            var url = new StringBuilder();
            url.Append(appPath).Append("/");
            url.Append(Controller).Append("/");
            if (!string.IsNullOrEmpty(Action))
            {
                url.Append(Action).Append("/");
            }
            if (paramters != null)
            {
                foreach (var o in paramters)
                {
                    if (o != null)
                    {
                        url.Append(HttpUtility.UrlEncode(o.ToString())).Append("/");
                    }
                }
            }
            url.Length--;
            if (WebSettings.UsingAspxPostfix)
            {
                url.Append(".aspx");
            }
            return url.ToString();
        }
    }
}
