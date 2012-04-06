using System;
using System.Web;
using Leafing.Core.Ioc;

namespace Leafing.Web.Mvc.Core
{
    [DependenceEntry, Implementation(1)]
    public class CookiesHandler
    {
        public static readonly CookiesHandler Instance = SimpleContainer.Get<CookiesHandler>();

        public virtual string this[string name]
        {
            get
            {
                var c = HttpContext.Current.Request.Cookies[name];
                return c == null ? null : c.Value;
            }
            set
            {
                SetCookies(HttpContext.Current.Request.Cookies[name], value, false);
                SetCookies(HttpContext.Current.Response.Cookies[name], value, true);
            }
        }

        private static void SetCookies(HttpCookie c, string value, bool throwException)
        {
            if (c == null)
            {
                if (throwException)
                {
                    throw new WebException("Unexpacted exception");
                }
                return;
            }
            c.Value = value;
        }

        public virtual void SetCookie(string key, string value, string domain, DateTime expires)
        {
            var cookie = new HttpCookie(key, value) {Expires = expires, Domain = domain, Path = "/"};
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
