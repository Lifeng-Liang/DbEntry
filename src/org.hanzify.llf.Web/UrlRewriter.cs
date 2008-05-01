
using System.Web;
using System.Web.UI;
using System.IO;

namespace Lephone.Web
{
    public class UrlRewriter : RailsDispatcher
    {
        protected override void InvokeAction(HttpContext context, string ControllerName, string[] ss)
        {
            string Action = ss.Length > 1 ? ss[1] : "list";
            // string Param = ss.Length > 2 ? ss[2] : null;

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
