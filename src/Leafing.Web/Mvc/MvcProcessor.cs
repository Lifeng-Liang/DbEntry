using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Leafing.Core;
using Leafing.Core.Ioc;
using Leafing.Web.Mvc.Core;

namespace Leafing.Web.Mvc
{
    [DependenceEntry, Implementation(1)]
    public class MvcProcessor
    {
        protected readonly ControllerInfo Controller;
        protected readonly ActionInfo Action;
        protected readonly object[] Parameters;

        public MvcProcessor()
        {
            var url = GetUrl();
            var ss = url.Split(ControllerFinder.Spliter, StringSplitOptions.RemoveEmptyEntries);
            Controller = GetControllerInfo(ss);
            Action = GetActionInfo(ss);
            Parameters = GetParameters(ss);
        }

        private string GetUrl()
        {
            var url = HttpContextHandler.Instance.AppRelativeCurrentExecutionFilePath;
            url = url.Substring(2);

            if (WebSettings.MvcPostfix != "")
            {
                if (url.EndsWith(WebSettings.MvcPostfix, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Substring(0, url.Length - WebSettings.MvcPostfix.Length);
                }
            }
            return url;
        }

        private ControllerInfo GetControllerInfo(string[] ss)
        {
            var controllerName = GetControllerName(ss);
            if (ControllerFinder.Controllers.ContainsKey(controllerName))
            {
                return ControllerFinder.Controllers[controllerName];
            }
            throw new WebException("Controller [{0}] not found!", controllerName);
        }

        private string GetControllerName(string[] ss)
        {
            var controllerName = (ss.Length == 0) ? "default" : HttpUtility.UrlDecode(ss[0]).ToLower();
            if (ss.Length == 1 && controllerName == "default.aspx")
            {
                controllerName = "default";
            }
            return controllerName;
        }

        private ActionInfo GetActionInfo(string[] ss)
        {
            var actionName = ss.Length > 1 ? HttpUtility.UrlDecode(ss[1]).ToLower() : Controller.DefaultAction;
            if (Controller.Actions.ContainsKey(actionName))
            {
                return Controller.Actions[actionName];
            }
            throw new WebException("Action [{1}.{0}] not found!", actionName, Controller.Name);
        }

        private object[] GetParameters(string[] ss)
        {
            var pis = Action.Method.GetParameters();
            var result = new object[pis.Length];
            if (ss.Length > 2)
            {
                var parameters = new List<string>();
                for (int i = 2; i < ss.Length; i++)
                {
                    parameters.Add(HttpUtility.UrlDecode(ss[i]));
                }
                if (ss.Length - 2 > pis.Length)
                {
                    throw new WebException("The count of paremeters for Action [{1}.{0}] is wrong!", Action.Name, Controller.Name);
                }
                for (int i = 0; i < pis.Length; i++)
                {
                    var type = pis[i].ParameterType;
                    if (i < parameters.Count)
                    {
                        object px = ChangeType(parameters[i], type);
                        result[i] = px;
                    }
                }
            }
            return result;
        }

        protected object ChangeType(string s, Type t)
        {
            if (t.IsValueType && string.IsNullOrEmpty(s))
            {
                return Util.GetEmptyValue(t);
            }
            return ClassHelper.ChangeType(s, t);
        }

        public virtual void Process()
        {
            var ctl = CreateControllerInstance();
            try
            {
                var ret = InvokeAction(ctl);
                if (ret is IView)
                {
                    ((IView)ret).Render();
                }
                else if (ret == null || (string)ret == "")
                {
                    var view = new PageView(Action.ViewName, ctl, Controller);
                    view.Render();
                }
                else
                {
                    HttpContextHandler.Instance.Redirect((string)ret, false);
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                OnException(ex, ctl);
            }
        }

        protected ControllerBase CreateControllerInstance()
        {
            var ctl = Controller.CreateInstance();
            if (ctl == null)
            {
                throw new WebException("The Controller must inherits from ControllerBase");
            }
            return ctl;
        }

        protected virtual object InvokeAction(ControllerBase ctl)
        {
            return Action.Method.Invoke(ctl, Parameters);
        }

        protected virtual void OnException(Exception exception, ControllerBase controller)
        {
            controller.OnException(exception);
        }
    }
}
