using System.Text;
using Leafing.Core;
using Leafing.Web.Mvc;
using Soaring.Biz.Helpers;
using Soaring.Biz.Models;

namespace Soaring.Biz.Controllers
{
    public class UserController : ControllerBase<User>
    {
        public string Login()
        {
            var vm = Bind<LoginViewModel>();
            if (vm == null || vm.Email == null || vm.Password == null)
            {
                return null;
            }
            var u = User.GetUserForLogin(vm.Email, vm.Password);
            if (u != null)
            {
                if (vm.RememberMe)
                {
                    var et = Util.Now.AddDays(30);
                    Cookies.SetCookie(LoginHelper.LoginCookie,
                                      u.SessionId, null, et);
                    u.SessionValidUntil = et;
                    u.Save();
                }
                else
                {
                    Cookies[LoginHelper.LoginCookie] = u.SessionId;
                    u.SessionValidUntil = Util.Now.AddDays(1);
                    u.Save();
                }
                return UrlTo<RequirementController>();
            }
            Flash.Warning = "用户名或密码错误";
            return null;
        }

        public string Logout(string url)
        {
            var user = LoginHelper.GetLoginUser();
            if(user != null)
            {
                user.ResetSessionId();
                user.Save();
            }
            LoginHelper.SetLoginId(null);
            return url ?? UrlTo<UserController>(p => p.Login());
        }

        public string Register()
        {
            var user = Bind<User>();
            if (user.Email.LikeNull() || user.Password.LikeNull() || user.Nick.LikeNull() || user.Email.IndexOf("@") < 0)
            {
                Flash.Warning = "Email密码以及显示名都是必填项";
                return null;
            }
            user.ResetSessionId();
            var validater = user.Validate();
            if(validater.IsValid)
            {
                user.Save();
                Flash.Notice = "用户创建成功";
                return UrlTo("user").Action("login");
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
