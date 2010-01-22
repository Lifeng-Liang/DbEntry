using System.Web;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class CookiesHandler
    {
        public static readonly CookiesHandler Instance = (CookiesHandler)ClassHelper.CreateInstance(WebSettings.CookiesHandler);

        public virtual string this[string name]
        {
            get
            {
                var c = HttpContext.Current.Request.Cookies[name];
                return c == null ? null : c.Value;
            }
            set
            {
                SetCookies(HttpContext.Current.Request.Cookies[name], value);
                SetCookies(HttpContext.Current.Response.Cookies[name], value);
            }
        }

        private void SetCookies(HttpCookie c, string value)
        {
            if(c == null)
            {
                throw new WebException("Unexpacted exception");
            }
            c.Value = value;
        }
    }
}
