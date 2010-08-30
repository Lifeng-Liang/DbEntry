using System;
using System.Text;
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
                    var cookie = new HttpCookie(Const.LoginCookie, 
                        User.SerializeToString(u.Email, password)) { Expires = DateTime.Now.AddDays(30) };
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

        public string Register([Bind]User user)
        {
            if (user.Email.LikeNull() || user.Password.LikeNull() || user.Nick.LikeNull() || user.Email.IndexOf("@") < 0)
            {
                Flash.Warning = "Email密码以及显示名都是必填项";
                return null;
            }
            var validater = user.Validate();
            if(validater.IsValid)
            {
                user.Save();
                Flash.Notice = "用户创建成功";
                return UrlTo.Controller("user").Action("login");
            }
            var sb = new StringBuilder();
            foreach (var message in validater.ErrorMessages)
            {
                sb.Append(message);
            }
            Flash.Warning = sb.ToString();
            return null;
        }
    }
}
