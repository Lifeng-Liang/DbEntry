
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Lephone.Web.Common;
using Lephone.Data;

namespace Lephone.Web
{
    public class PageBase : Page
    {
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();
        protected internal string ControllerName;

        public PageBase()
        {
        }

        protected internal string LinkTo(string Title, string Controller, string Action, string Paramter)
        {
            if(string.IsNullOrEmpty(Title))
            {
                throw new DbEntryException("Title can not be null or empty.");
            }
            StringBuilder url = new StringBuilder("<a href=\"");
            url.Append(Request.ApplicationPath).Append("/");
            if (!string.IsNullOrEmpty(Controller))
            {
                url.Append(Controller).Append("/");
            }
            else
            {
                url.Append(ControllerName).Append("/");
            }
            if (!string.IsNullOrEmpty(Action))
            {
                url.Append(Action).Append("/");
            }
            if(!string.IsNullOrEmpty(Paramter))
            {
                url.Append(Paramter).Append("/");
            }
            url.Length--;
            if (WebSettings.UsingAspxPostfix)
            {
                url.Append(".aspx");
            }
            url.Append("\">");
            url.Append(Title);
            url.Append("</a>");
            return url.ToString();
        }
    }
}
