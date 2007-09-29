
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Util;

namespace Lephone.Web
{
    public class DefaultController : ControllerBase
    {
        private static string template = ResourceHelper.ReadToEnd(typeof(DefaultController), "Default.htm");

        public void List()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in HttpDispatcher.ctls.Keys)
            {
                sb.Append("<li><a href=\"").Append(s).Append("\">").Append(s).Append("</a></li>\n");
            }
            ctx.Response.Write(string.Format(template, sb));
        }
    }
}
