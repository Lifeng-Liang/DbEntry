using System;
using System.Threading;
using System.Web;
using Leafing.Core.Ioc;
using Leafing.Web.Mvc;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web
{
    public sealed class MvcDispatcher : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var p = SimpleContainer.Get<MvcProcessor>();
                p.Process();
            }
            catch(ThreadAbortException)
            {
                throw;
            }
            catch(Exception ex)
            {
                ControllerHelper.OnException(ex);
            }
        }
    }
}
