﻿using System.Web;
using Lephone.Core;

namespace Lephone.Web.Mvc
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
    }
}


