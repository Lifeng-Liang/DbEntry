
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;

namespace Lephone.Web
{
    public class UrlRewriter : RailsDispatcher
    {
        protected override void CallController(HttpContext context, string ControllerName, string Action, string Param)
        {
            string vp = context.Request.ApplicationPath + "/Views/" + ControllerName + "/" + Action + ".aspx";
            string pp = context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                Page p = factory.GetHandler(context, context.Request.RequestType, vp, pp) as Page;
                if (p != null)
                {
                    ((IHttpHandler)p).ProcessRequest(context);
                }
            }
        }
    }
}
