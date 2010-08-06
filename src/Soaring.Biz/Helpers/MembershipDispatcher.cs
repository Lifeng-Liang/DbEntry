using System.Collections.Generic;
using System.Web;
using Lephone.Web;
using Lephone.Web.Mvc;
using Soaring.Biz.Models;

namespace Soaring.Biz.Helpers
{
    public class MembershipDispatcher : MvcDispatcher
    {
        private readonly static Dictionary<string, List<string>> FreePages;

        static MembershipDispatcher()
        {
            FreePages = new Dictionary<string, List<string>>
                            {
                                {"user", new List<string> {"login", "logout", "register"}},
                            };
        }

        protected internal SessionHandler Session = new SessionHandler();

        protected override string InvokeAction(HttpContext context, string controllerName, string actionName, string[] ss, ControllerBase ctl, ControllerInfo ci)
        {
            if (FreePages.ContainsKey(controllerName))
            {
                var actions = FreePages[controllerName];
                if (actions != null && actions.Contains(actionName))
                {
                    return base.InvokeAction(context, controllerName, actionName, ss, ctl, ci);
                }
            }

            var user = this.GetLoginUser();
            if (user == null)
            {
                var luc = context.Request.Cookies[Const.LoginCookie];
                if (luc != null)
                {
                    user = User.DeserializeFromString(luc.Value);
                }
                if (null != user)
                {
                    Session[Const.LoginSession] = user;
                }
            }
            if(user == null)
            {
                context.Response.Redirect(new UrlToInfo("user").Action("login"), true);
            }
            return base.InvokeAction(context, controllerName, actionName, ss, ctl, ci);
        }
    }
}
