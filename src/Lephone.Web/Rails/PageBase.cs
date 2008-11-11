using System.Collections.Generic;
using System.Text;
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

        protected internal string LinkTo(string title, string controller, string action, string addon, params object[] paramters)
        {
            return LinkTo(Request.ApplicationPath, title, controller, action, addon, paramters);
        }

        internal string LinkTo(string appPath, string title, string controller, string action, string addon, params object[] paramters)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new DataException("title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                UrlTo(appPath, string.IsNullOrEmpty(controller) ? ControllerName : controller, action, paramters),
                title,
                addon == null ? "" : " " + addon);
            return ret;
        }

        protected internal string UrlTo(string action, params object[] paramters)
        {
            return UrlTo(ControllerName, action, paramters);
        }

        protected internal string UrlTo(string controller, string action, params object[] paramters)
        {
            return UrlTo(Request.ApplicationPath, controller, action, paramters);
        }

        internal static string UrlTo(string appPath, string controller, string action, params object[] paramters)
        {
            var url = new StringBuilder();
            url.Append(appPath).Append("/");
            url.Append(controller).Append("/");
            if (!string.IsNullOrEmpty(action))
            {
                url.Append(action).Append("/");
            }
            if (paramters != null)
            {
                foreach (var o in paramters)
                {
                    url.Append(o).Append("/");
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
