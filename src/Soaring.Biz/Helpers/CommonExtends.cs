using Lephone.Web.Mvc;
using Soaring.Biz.Models;

namespace Soaring.Biz.Helpers
{
    public static class CommonExtends
    {
        public static bool LikeNull(this string s)
        {
            if (s == null)
            {
                return true;
            }
            if (s.Trim() == "")
            {
                return true;
            }
            return false;
        }

        public static User GetLoginUser(this MembershipDispatcher page)
        {
            return GetLoginUser(page.Session);
        }

        public static User GetLoginUser(this ControllerBase controller)
        {
            return GetLoginUser(controller.Session);
        }

        public static User GetLoginUser(this MasterPageBase page)
        {
            return GetLoginUser(page.Session);
        }

        public static User GetLoginUser(this PageBase page)
        {
            return GetLoginUser(page.Session);
        }

        private static User GetLoginUser(SessionHandler session)
        {
            var u = session[Const.LoginSession];
            if (u != null && u is User)
            {
                return (User)u;
            }
            return null;
        }
    }
}
