using System.Security.Cryptography;
using System.Text;
using Leafing.Core.Text;
using Leafing.Web.Mvc.Core;
using Soaring.Biz.Models;

namespace Soaring.Biz.Helpers
{
    public static class LoginHelper
    {
        public static readonly string LoginCookie = "Soaring.LoginId";

        public static string GetHashedPassword(string password)
        {
            var hash = SHA512.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            var ret = Base32StringCoding.Decode(bytes);
            return ret;
        }

        public static User GetLoginUser()
        {
            return User.FindBySessionId(GetLoginId());
        }

        public static string GetLoginId()
        {
            return CookiesHandler.Instance[LoginCookie];
        }

        public static void SetLoginId(string sid)
        {
            CookiesHandler.Instance[LoginCookie] = sid;
        }
    }
}
