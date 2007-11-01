
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using Lephone.Web.Common;
using Lephone.Data;

namespace Lephone.Web.Rails
{
    public class PageBase : Page
    {
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();
        protected internal string ControllerName;
        protected internal string ActionName;

        protected FlashBox flash = new FlashBox();

        public PageBase()
        {
        }

        protected internal void Print(object o)
        {
            Response.Write(o);
        }

        protected internal void Print(string s)
        {
            Response.Write(s);
        }

        protected internal string LinkTo(string Title, string Controller, string Action, object Paramter)
        {
            return LinkTo(Title, Controller, Action, Paramter, null);
        }

        protected internal string LinkTo(string Title, string Controller, string Action, object Paramter, string AddOn)
        {
            return LinkTo(Request.ApplicationPath, Title, Controller, Action, Paramter, AddOn);
        }

        internal string LinkTo(string AppPath, string Title, string Controller, string Action, object Paramter, string AddOn)
        {
            string ParamterStr = (Paramter == null) ? null : Paramter.ToString();
            if (string.IsNullOrEmpty(Title))
            {
                throw new DataException("Title can not be null or empty.");
            }
            string ret = string.Format("<a href=\"{0}\"{2}>{1}</a>",
                UrlTo(AppPath,
                string.IsNullOrEmpty(Controller) ? ControllerName : Controller,
                Action, ParamterStr), Title, AddOn == null ? "" : " " + AddOn);
            return ret;
        }

        protected internal string UrlTo(string Action, string Paramter)
        {
            return UrlTo(ControllerName, Action, Paramter);
        }

        protected internal string UrlTo(string Controller, string Action, string Paramter)
        {
            return UrlTo(Request.ApplicationPath, Controller, Action, Paramter);
        }

        internal static string UrlTo(string AppPath, string Controller, string Action, string Paramter)
        {
            StringBuilder url = new StringBuilder();
            url.Append(AppPath).Append("/");
            url.Append(Controller).Append("/");
            if (!string.IsNullOrEmpty(Action))
            {
                url.Append(Action).Append("/");
            }
            if (!string.IsNullOrEmpty(Paramter))
            {
                url.Append(Paramter).Append("/");
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
