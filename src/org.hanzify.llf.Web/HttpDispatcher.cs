
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;
using Lephone.Util;

namespace Lephone.Web
{
    public class HttpDispatcher : IHttpHandler
    {
        protected PageHandlerFactory factory = ClassHelper.CreateInstance<PageHandlerFactory>();
        internal static Dictionary<string, Type> ctls;
        private static char[] spliter = new char[] { '/' };

        static HttpDispatcher()
        {
            Dictionary<string, object> excepted = CreateExcepted(
                "Lephone.Data", "Lephone.Util", "Lephone.Web",
                "mscorlib", "System", "System.Data", "System.Web", "System.Xml",
                "System.Web.Mobile", "System.resources");

            ctls = new Dictionary<string, Type>();
            ctls["default"] = typeof(DefaultController);
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                string s = a.FullName.Split(',')[0];
                if(!excepted.ContainsKey(s))
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(ControllerBase)))
                        {
                            string tn = t.Name;
                            if (tn.EndsWith("Controller"))
                            {
                                tn = tn.Substring(0, tn.Length - 10);
                            }
                            ctls[tn.ToLower()] = t;
                        }
                    }
                }
            }
        }

        private static Dictionary<string, object> CreateExcepted( params string[] ss)
        {
            Dictionary<string, object> excepted = new Dictionary<string,object>();
            foreach (string s in ss)
            {
                excepted.Add(s, 0);
            }
            return excepted;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request.RawUrl;
            url = url.Substring(context.Request.ApplicationPath.Length);
            
            if (url.ToLower().EndsWith(".aspx"))
            {
                url = url.Substring(0, url.Length - 5);
            }

            string[] ss = url.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
            string ControllerName;

            if(ss.Length== 0)
            {
                ControllerName = "default";
            }
            else if (ss.Length > 2)
            {
                throw new NotSupportedException("Not supported more than 2 paramters now.");
            }
            else
            {
                ControllerName = ss[0].ToLower();
            }

            if (ctls.ContainsKey(ControllerName))
            {
                string op = ss.Length > 1 ? ss[1] : "list";
                CallController(context, ControllerName, op);
                return;
            }
            context.Response.StatusCode = 404;
        }

        protected virtual void CallController(HttpContext context, string ControllerName, string op)
        {
            Type t = ctls[ControllerName];
            ControllerBase ctl = ClassHelper.CreateInstance(t) as ControllerBase;
            ctl.ctx = context;
            MethodInfo mi = t.GetMethod(op, ClassHelper.InstancePublic | BindingFlags.IgnoreCase);
            mi.Invoke(ctl, new object[] { });

            string vp = context.Request.ApplicationPath + "/Views/" + ControllerName + "/" + op + ".aspx";
            string pp = context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                PageBase p = factory.GetHandler(context, context.Request.RequestType, vp, pp) as PageBase;
                if (p != null)
                {
                    p.bag = ctl.bag;
                    p.ControllerName = ControllerName;
                    ((IHttpHandler)p).ProcessRequest(context);
                }
                else
                {
                    context.Response.Write("<b><big>The template page must inherits from PageBase!!!</big></b>");
                }
            }
        }
    }
}
