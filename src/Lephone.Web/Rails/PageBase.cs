using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class PageBase : Page
    {
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();
        protected internal string ControllerName;
        protected internal string ActionName;

        protected FlashBox flash = new FlashBox();

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
            return LinkTo(Request.ApplicationPath, args, paramters);
        }

        internal string LinkTo(string appPath, LTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Title))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                UrlTo(appPath, args.ToUTArgs(), paramters),
                args.Title,
                args.Addon == null ? "" : " " + args.Addon);
            return ret;
        }

        protected internal string UrlTo(UTArgs args, params object[] paramters)
        {
            return UrlTo(Request.ApplicationPath, args, paramters);
        }

        internal string UrlTo(string appPath, UTArgs args, params object[] paramters)
        {
            if (string.IsNullOrEmpty(args.Controller))
            {
                args.Controller = ControllerName;
            }
            return UrlTo(appPath, args.Controller, args.Action, paramters);
        }

        public static string UrlTo(string appPath, string Controller, string Action, params object[] paramters)
        {
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
