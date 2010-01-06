using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.IO;
using System.Web.Compilation;
using Lephone.Util;
using Lephone.Web.Rails;

namespace Lephone.Web
{
    public class RailsDispatcher : IHttpHandler
    {
        protected internal static Dictionary<string, ControllerInfo> Ctls;
        protected static readonly char[] Spliter = new[] { '/' };
        protected static readonly Type CbType = typeof(ControllerBase);

        static RailsDispatcher()
        {
            Ctls = new Dictionary<string, ControllerInfo>();
            Ctls["default"] = new ControllerInfo(typeof(DefaultController));
            if (WebSettings.ControllerAssembly == "")
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a.FullName == null) { continue; }
                    string s = a.FullName.Split(',')[0];
                    if (!s.StartsWith("System.") && CouldBeControllerAssemebly(s))
                    {
                        SearchControllers(a);
                    }
                }
            }
            else
            {
                var a = Assembly.Load(WebSettings.ControllerAssembly);
                SearchControllers(a);
            }
        }

        protected static void SearchControllers(Assembly a)
        {
            foreach (Type t in a.GetTypes())
            {
                if (t.IsSubclassOf(CbType))
                {
                    string tn = t.Name;
                    if (tn.EndsWith("Controller"))
                    {
                        tn = tn.Substring(0, tn.Length - 10);
                    }
                    Ctls[tn.ToLower()] = new ControllerInfo(t);
                }
            }
        }

        protected static bool CouldBeControllerAssemebly(string s)
        {
            switch(s)
            {
                case "Lephone.Data":
                case "Lephone.Util":
                case "Lephone.Web":
                case "mscorlib":
                case "System":
                    return false;
                default:
                    return true;
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            string url = context.Request.AppRelativeCurrentExecutionFilePath;
            url = url.Substring(2);
            
            if(WebSettings.RailsPostfix != "")
            {
                if (url.ToLower().EndsWith(WebSettings.RailsPostfix))
                {
                    url = url.Substring(0, url.Length - WebSettings.RailsPostfix.Length);
                }
            }

            string[] ss = url.Split(Spliter, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < ss.Length; i++)
            {
                ss[i] = HttpUtility.UrlDecode(ss[i]);
            }

            string controllerName = (ss.Length == 0) ? "default" : ss[0].ToLower();
            if(ss.Length == 1 && controllerName == "default.aspx")
            {
                controllerName = "default";
            }

            if (Ctls.ContainsKey(controllerName))
            {
                Process(context, controllerName, ss);
                return;
            }

            OnControllerNotFound(controllerName, context);
        }

        protected virtual void Process(HttpContext context, string controllerName, string[] ss)
        {
            // Invoke Controller
            var ci = Ctls[controllerName];
            var ctl = ClassHelper.CreateInstance(ci.Type) as ControllerBase;
            if (ctl == null)
            {
                throw new WebException("The Controller must inherits from ControllerBase");
            }
            ctl.Ctx = context;

            try
            {
                string actionName = ss.Length > 1 ? ss[1] : ci.DefaultAction;
                string viewName = InvokeAction(context, controllerName, actionName.ToLower(), ss, ctl, ci);
                if(!viewName.IsNullOrEmpty())
                {
                    InvokeView(context, viewName, ctl, ci);
                }
            }
            catch (Exception ex)
            {
                OnException(ex, ctl);
            }
        }

        protected virtual string InvokeAction(HttpContext context, string controllerName, string actionName, string[] ss, ControllerBase ctl, ControllerInfo ci)
        {
            MethodInfo mi = GetMethodInfo(ci.Type, actionName);
            if (mi == null)
            {
                throw new WebException(string.Format("Action {0} doesn't exist!!!", actionName));
            }
            List<object> parameters = GetParameters(ss, mi);
            object ret = CallAction(mi, ctl, parameters.ToArray()) ?? "";

            var va = ClassHelper.GetAttribute<ViewAttribute>(mi, false);
            string viewName = (va == null) ? mi.Name : va.ViewName;

            if(string.IsNullOrEmpty(ret.ToString()))
            {
                return viewName;
            }
            context.Response.Redirect(ret.ToString(), false);
            return null;
        }

        protected virtual void InvokeView(HttpContext context, string viewName, ControllerBase ctl, ControllerInfo ci)
        {
            PageBase p = CreatePage(context, ci, viewName);
            if (p != null)
            {
                InitViewPage(ci.ControllerName, ctl, viewName, p);
                ((IHttpHandler)p).ProcessRequest(context);
            }
        }

        protected virtual void OnException(Exception exception, ControllerBase controller)
        {
            controller.OnException(exception);
        }

        protected virtual void OnControllerNotFound(string controllerName, HttpContext context)
        {
            ControllerHelper.OnException(new WebException("Controller [{0}] not found!", controllerName), context);
        }

        protected static MethodInfo GetMethodInfo(Type t, string actionName)
        {
            MethodInfo mi = t.GetMethod(actionName, ClassHelper.InstancePublic | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
            if (mi != null) return mi;
            return t.GetMethod(actionName, ClassHelper.InstancePublic | BindingFlags.IgnoreCase);
        }

        protected static void InitViewPage(string controllerName, ControllerBase ctl, string actionName, PageBase p)
        {
            p.Bag = ctl.Bag;
            p.ControllerName = controllerName.ToLower();
            p.ActionName = actionName.ToLower();
            p.InitFields();
        }

        protected static List<object> GetParameters(string[] ss, MethodInfo mi)
        {
            ParameterInfo[] pis = mi.GetParameters();
            var parameters = new List<object>();
            for (int i = 0; i < pis.Length; i++)
            {
                if (i + 2 < ss.Length)
                {
                    if(pis[i].ParameterType.IsArray)
                    {
                        ProcessArray(parameters, ss, i + 2, pis, i);
                        break;
                    }
                    object px = ChangeType(ss[i + 2], pis[i].ParameterType);
                    parameters.Add(px);
                }
                else
                {
                    parameters.Add(null);
                }
            }
            return parameters;
        }

        public static void ProcessArray(List<object> parameters, string[] ss, int startIndex, ParameterInfo[] pis, int curIndex)
        {
            var list = new List<string>();
            int valuesCount = pis.Length - curIndex - 1;
            for (int i = startIndex; i < ss.Length - valuesCount; i++)
            {
                list.Add(ss[i]);
            }

            var values = new List<object>();
            int nIndex = ss.Length - valuesCount;
            for (int i = 0; i < valuesCount; i++)
            {
                Type t = pis[i + curIndex + 1].ParameterType;
                object px = ChangeType(ss[nIndex + i], t);
                values.Add(px);
            }

            parameters.Add(list.ToArray());
            parameters.AddRange(values);
        }

        protected virtual PageBase CreatePage(HttpContext context, ControllerInfo ci, string actionName)
        {
            string vp = "/Views/" + ci.ControllerName + "/" + actionName + ".aspx";
            if(context.Request.ApplicationPath != "/")
            {
                vp = context.Request.ApplicationPath + vp;
            }
            string pp = context.Server.MapPath(vp);
            if (File.Exists(pp))
            {
                object o = BuildManager.CreateInstanceFromVirtualPath(vp, typeof(object));
                if (o == null || !(o is PageBase))
                {
                    throw new WebException("The template page must inherits from PageBase!!!");
                }
                return (PageBase)o;
            }
            if (ci.IsScaffolding)
            {
                Type tt = GetScaffoldingType(ci.Type);
                if(string.IsNullOrEmpty(WebSettings.ScaffoldingMasterPage))
                {
                    return new ScaffoldingViews(ci, tt, context);
                }
                return new ScaffoldingViewsWithMaster(ci, tt, context);
            }
            if (ci.Type == typeof(DefaultController))
            {
                return null;
            }
            throw new WebException(string.Format("The action {0} don't have view file!!!", actionName));
        }

        protected static Type GetScaffoldingType(Type t)
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

        protected static object CallAction(MethodBase mi, ControllerBase c, object[] ps)
        {
            object o = c.OnBeforeAction(mi.Name);
            if (o != null) return o;
            o = mi.Invoke(c, ps);
            if (o != null) return o;
            o = c.OnAfterAction(mi.Name);
            return o;
        }

        protected static object ChangeType(string s, Type t)
        {
            if(t.IsValueType && string.IsNullOrEmpty(s))
            {
                return CommonHelper.GetEmptyValue(t);
            }
            return ClassHelper.ChangeType(s, t);
        }
    }
}
