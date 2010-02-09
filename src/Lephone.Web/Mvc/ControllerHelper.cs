using System;
using System.Web;
using Lephone.Util;

namespace Lephone.Web.Mvc
{
    public static class ControllerHelper
    {
        private static readonly string ErrorTemplate = ResourceHelper.ReadToEnd(typeof(ControllerBase), "Mvc.Error.htm");

        public static void OnException(Exception ex, HttpContext ctx)
        {
            OnException(ex, ctx, 404);
        }

        public static void OnException(Exception ex, HttpContext ctx, int statusCode)
        {
            Exception e = ex.InnerException ?? ex;
            string title = ctx.Server.HtmlEncode(e.Message);
            string text = e.ToString();
            string result = string.Format(ErrorTemplate, title, text);
            ctx.Response.Write(result);
            ctx.Response.StatusCode = statusCode;
        }

        public static object ChangeType(object value, Type t)
        {
            if (t.IsEnum)
            {
                return Enum.Parse(t, value.ToString());
            }
            if (t == typeof(bool))
            {

                if (value == null) return false;
                switch (value.ToString().ToLower())
                {
                    case "on":
                    case "true":
                    case "1":
                        return true;
                    case "off":
                    case "false":
                    case "0":
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return ClassHelper.ChangeType(value, t);
        }
    }
}


