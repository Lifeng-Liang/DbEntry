using System;
using System.Web;
using Leafing.Core;

namespace Leafing.Web.Mvc.Core
{
    public static class ControllerHelper
    {
        private static readonly string ErrorTemplate = ResourceHelper.ReadToEnd(typeof(ControllerBase), "Mvc.Core.Error.htm");

        public static void OnException(Exception ex)
        {
            OnException(ex, 404);
        }

        public static void OnException(Exception ex, int statusCode)
        {
            Exception e = ex.InnerException ?? ex;
            string title = HttpUtility.HtmlEncode(e.Message);
            string text = e.ToString();
            string result = string.Format(ErrorTemplate, title, text);
            HttpContextHandler.Instance.Write(result);
            HttpContextHandler.Instance.StatusCode = statusCode;
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


