using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.IO;
using Lephone.Data.Definition;
using Lephone.Util;
using Lephone.Web.Rails;

namespace Lephone.Web
{
    public class RailsDispatcher : IHttpHandler
    {
        protected PageHandlerFactory factory = ClassHelper.CreateInstance<PageHandlerFactory>();
        internal static Dictionary<string, Type> ctls;
        private static readonly char[] spliter = new[] { '/' };

        static RailsDispatcher()
        {
            Dictionary<string, object> excepted = CreateExcepted(
                "Lephone.Data", "Lephone.Util", "Lephone.Web",
                "mscorlib", "System", "System.Data", "System.Web", "System.Xml",
                "System.Web.Mobile", "System.resources", "System.configuration");

            ctls = new Dictionary<string, Type>();
            ctls["default"] = typeof(DefaultController);
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(a.FullName == null) { continue; }
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
            var excepted = new Dictionary<string,object>();
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
            for(int i = 0; i < ss.Length; i++)
            {
                ss[i] = HttpUtility.UrlDecode(ss[i]);
            }

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
            var ctl = ClassHelper.CreateInstance(t) as ControllerBase;
            if(ctl == null)
            {
                throw new WebException("The Controller must inherits from ControllerBase");
            }
            ctl.ctx = context;

            try
            {
                ControllerInfo ci = ControllerInfo.GetInstance(t);
                string ActionName = ss.Length > 1 ? ss[1] : ci.DefaultAction;
                MethodInfo mi = t.GetMethod(ActionName, ClassHelper.InstancePublic | BindingFlags.IgnoreCase);
                if (mi == null)
                {
                    throw new WebException(string.Format("Action {0} doesn't exist!!!", ActionName));
                }

                List<object> parameters = GetParameters(ss, mi);
                object ret = CallAction(mi, ctl, parameters.ToArray()) ?? "";
                if(string.IsNullOrEmpty(ret.ToString()))
                {
                    // Invoke Viewer
                    PageBase p = CreatePage(context, ci, t, ControllerName, ActionName);
                    if (p != null)
                    {
                        InitViewPage(ControllerName, ctl, ActionName, p);
                        ((IHttpHandler)p).ProcessRequest(context);
                        factory.ReleaseHandler(p);
                    }
                }
                else
                {
                    context.Response.Redirect(ret.ToString());
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

        private static void InitViewPage(string ControllerName, ControllerBase ctl, string ActionName, PageBase p)
        {
            p.bag = ctl.bag;
            p.ControllerName = ControllerName;
            p.ActionName = ActionName;
            // init fields by the bag variables\
            var infoList = p.GetType().GetFields(ClassHelper.InstancePublic | BindingFlags.DeclaredOnly);
            foreach (FieldInfo info in infoList)
            {
                if (!ClassHelper.HasAttribute<ExcludeAttribute>(info, false))
                {
                    object value = p.bag[info.Name];
                    info.SetValue(p, value);
                }
            }
        }

        private static List<object> GetParameters(string[] ss, MethodInfo mi)
        {
            ParameterInfo[] pis = mi.GetParameters();
            var parameters = new List<object>();
            for (int i = 0; i < pis.Length; i++)
            {
                if (i + 2 < ss.Length)
                {
                    object px;
                    if(pis[i].ParameterType.IsArray)
                    {
                        px = GetArray(ss, i + 2);
                    }
                    else
                    {
                        px = ChangeType(ss[i + 2], pis[i].ParameterType);
                    }
                    parameters.Add(px);
                }
                else
                {
                    parameters.Add(null);
                }
            }
            return parameters;
        }

        private static string[] GetArray(string[] ss, int startIndex)
        {
            var list = new List<string>();
            for (int i = startIndex; i < ss.Length; i++)
            {
                list.Add(ss[i]);
            }
            return list.ToArray();
        }

        private PageBase CreatePage(HttpContext context, ControllerInfo ci, Type t, string ControllerName, string ActionName)
        {
            string vp = context.Request.ApplicationPath + "/Views/" + ControllerName + "/" + ActionName + ".aspx";
            string pp = context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                object o = factory.GetHandler(context, context.Request.RequestType, vp, pp);
                if (o == null)
                {
                    throw new WebException("The template page must inherits from PageBase!!!");
                }
                return o as PageBase;
            }
            if (ci.IsScaffolding)
            {
                Type tt = GetScaffoldingType(t);
                return new ScaffoldingViews(tt, context);
            }
            if (t == typeof(DefaultController))
            {
                return null;
            }
            throw new WebException(string.Format("The action {0} don't have view file!!!", ActionName));
        }

        private static Type GetScaffoldingType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ControllerBase<>))
            {
                return t.GetGenericArguments()[0];
            }
            if (t.BaseType == typeof(object))
            {
                throw new WebException("System Error!");
            }
            return GetScaffoldingType(t.BaseType);
        }

        private static object CallAction(MethodBase mi, ControllerBase c, object[] ps)
        {
            c.OnBeforeAction(mi.Name);
            object o = mi.Invoke(c, ps);
            c.OnAfterAction(mi.Name);
            return o;
        }

        private static object ChangeType(string s, Type t)
        {
            if(t.IsValueType && string.IsNullOrEmpty(s))
            {
                return CommonHelper.GetEmptyValue(t);
            }
            return ClassHelper.ChangeType(s, t);
        }
    }
}
