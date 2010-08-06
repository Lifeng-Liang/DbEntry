using System;
using System.Web;
using Lephone.Web.Mvc;
using Soaring.Biz.Helpers;
using Soaring.Biz.Models;

namespace Soaring.Biz.Controllers
{
    public class UserController : ControllerBase<User>
    {
        public string Login([Bind]string email, [Bind]string password, [Bind]bool rememberme)
        {
            if (email == null || password == null)
            {
                return null;
            }
            var u = User.GetUserForLogin(email, password);
            if (u != null)
            {
                Session[Const.LoginSession] = u;
                if (rememberme)
                {
                    var cookie = new HttpCookie(Const.LoginCookie, User.SerializeToString(u.Email, password)) { Expires = DateTime.Now.AddDays(30) };
                    Ctx.Response.Cookies.Add(cookie);
                }
                return UrlTo.Controller("requirement");
            }
            Flash.Warning = "用户名或密码错误";
            return null;
        }

        public string Logout(string url)
        {
            Session[Const.LoginSession] = null;
            var luc = Ctx.Request.Cookies[Const.LoginCookie];
            if (luc != null)
            {
                luc.Expires = DateTime.Now.AddDays(-1);
                luc.Value = "";
                Ctx.Response.Cookies.Set(luc);
            }
            return url ?? UrlTo.Controller("user").Action("login");
        }

        public void Register()
        {
        }
    }
}
