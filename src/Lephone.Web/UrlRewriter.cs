using System.Web;
using System.Web.UI;
using System.IO;

namespace Lephone.Web
{
    public class UrlRewriter : RailsDispatcher
    {
        protected override void ProcessAction(HttpContext context, string controllerName, string[] ss)
        {
            string action = ss.Length > 1 ? ss[1] : "list";
            // string Param = ss.Length > 2 ? ss[2] : null;

            string vp = context.Request.ApplicationPath + "/Views/" + controllerName + "/" + action + ".aspx";
            string pp = context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                var p = Factory.GetHandler(context, context.Request.RequestType, vp, pp) as Page;
                if (p != null)
                {
                    ((IHttpHandler)p).ProcessRequest(context);
                }
            }
        }
    }
}
