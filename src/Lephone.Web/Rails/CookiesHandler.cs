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
                var c = HttpContext.Current.Response.Cookies[name];
                if(c == null)
                {
                    throw new WebException("Unexpacted exception");
                }
                c.Value = value;
            }
        }
    }
}
