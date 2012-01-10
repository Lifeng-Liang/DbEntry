using System;
using System.Text;

namespace Leafing.Web.Mvc.Core
{
    public class FlashHandler
    {
        public string this[string name]
        {
            get
            {
                var n = WebSettings.FlashPrefix + name;
                var o = CookiesHandler.Instance[n];
                if(o != null)
                {
                    CookiesHandler.Instance[n] = null;
                    return Encoding.Unicode.GetString(Convert.FromBase64String(o));
                }
                return null;
            }
            set
            {
                var o = Convert.ToBase64String(Encoding.Unicode.GetBytes(value));
                CookiesHandler.Instance[WebSettings.FlashPrefix + name] = o;
            }
        }

        public string Tip
        {
            get { return this["Tip"]; }
            set { this["Tip"] = value; }
        }

        public string Notice
        {
            get { return this["Notice"]; }
            set { this["Notice"] = value; }
        }

        public string Warning
        {
            get { return this["Warning"]; }
            set { this["Warning"] = value; }
        }
    }
}


