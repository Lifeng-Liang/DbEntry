
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;
using Lephone.Data;
using Lephone.Util;
using Lephone.Web.Rails;

namespace Lephone.Web
{
    public class RailsDispatcher : IHttpHandler
    {
        protected PageHandlerFactory factory = ClassHelper.CreateInstance<PageHandlerFactory>();
        internal static Dictionary<string, Type> ctls;
        private static char[] spliter = new char[] { '/' };

        static RailsDispatcher()
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
            string url = context.Request.AppRelativeCurrentExecutionFilePath;
            url = url.Substring(2);
            
            if (url.ToLower().EndsWith(".aspx"))
            {
                url = url.Substring(0, url.Length - 5);
            }

            string[] ss = url.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
            string ControllerName = (ss.Length == 0) ? "default" : ss[0].ToLower();

            if (ctls.ContainsKey(ControllerName))
            {
                InvokeAction(context, ControllerName, ss);
                return;
            }
            context.Response.StatusCode = 404;
        }

        protected virtual void InvokeAction(HttpContext context, string ControllerName, string[] ss)
        {
            // Invoke Controller
            Type t = ctls[ControllerName];
            ControllerBase ctl = ClassHelper.CreateInstance(t) as ControllerBase;
            ctl.ctx = context;

            try
            {
                string Action = ss.Length > 1 ? ss[1] : ControllerInfo.GetInstance(t).DefaultAction;
                MethodInfo mi = t.GetMethod(Action, ClassHelper.InstancePublic | BindingFlags.IgnoreCase);
                if (mi == null)
                {
                    throw new WebException(string.Format("Action {0} doesn't exist!!!", Action));
                }

                ParameterInfo[] pis = mi.GetParameters();
                List<object> parameters = new List<object>();
                for (int i = 0; i < pis.Length; i++)
                {
                    if (i + 2 < ss.Length)
                    {
                        object px = ChangeType(ss[i + 2], pis[i].ParameterType);
                        parameters.Add(px);
                    }
                    else
                    {
                        parameters.Add(null);
                    }
                }
                CallAction(mi, ctl, parameters.ToArray());
                // Invoke Viewer
                string vp = context.Request.ApplicationPath + "/Views/" + ControllerName + "/" + Action + ".aspx";
                string pp = context.Server.MapPath(vp);
                if (File.Exists(pp))
                {
                    PageBase p = factory.GetHandler(context, context.Request.RequestType, vp, pp) as PageBase;
                    if (p != null)
                    {
                        p.bag = ctl.bag;
                        p.ControllerName = ControllerName;
                        ((IHttpHandler)p).ProcessRequest(context);
                        factory.ReleaseHandler((IHttpHandler)p);
                    }
                    else
                    {
                        throw new WebException("The template page must inherits from PageBase!!!");
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                ctl.OnException(ex);
            }
            catch (WebException ex)
            {
                ctl.OnException(ex);
            }
        }

        private void CallAction(MethodInfo mi, ControllerBase c, object[] ps)
        {
            c.OnBeforeAction(mi.Name);
            mi.Invoke(c, ps);
            c.OnAfterAction(mi.Name);
        }

        private object ChangeType(string s, Type t)
        {
            if(t.IsValueType && string.IsNullOrEmpty(s))
            {
                return CommonHelper.GetEmptyValue(t);
            }
            return Convert.ChangeType(s, t);
        }
    }
}
