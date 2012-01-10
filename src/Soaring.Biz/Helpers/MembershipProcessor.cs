using System.Collections.Generic;
using Leafing.Core.Ioc;
using Leafing.Web.Mvc;
using Leafing.Web.Mvc.Core;

namespace Soaring.Biz.Helpers
{
    [Implementation(3)]
    public class MembershipProcessor : MvcProcessor
    {
        private readonly static Dictionary<string, List<string>> FreePages;

        static MembershipProcessor()
        {
            FreePages = new Dictionary<string, List<string>>
                            {
                                {"user", new List<string> {"login", "logout", "register"}},
                            };
        }

        protected override object InvokeAction(ControllerBase ctl)
        {
            if (FreePages.ContainsKey(Controller.LowerName))
            {
                var actions = FreePages[Controller.LowerName];
                if (actions != null && actions.Contains(Action.LowerName))
                {
                    return base.InvokeAction(ctl);
                }
            }

            var user = LoginHelper.GetLoginUser();
            if (user == null)
            {
                HttpContextHandler.Instance.Redirect(new UrlToInfo("user").Action("login"), true);
            }
            return base.InvokeAction(ctl);
        }
    }
}
