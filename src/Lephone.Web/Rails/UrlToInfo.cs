using System.Collections;
using System.Text;
using System.Web;

namespace Lephone.Web.Rails
{
    public class UrlToInfo
    {
        public class UrlTo
        {
            private readonly string _controllerName;

            public UrlTo(string controllerName)
            {
                this._controllerName = controllerName;
            }

            public UrlToInfo Controller(string name)
            {
                var result = new UrlToInfo { _controller = name };
                return result;
            }

            public UrlToInfo Action(string name)
            {
                var result = new UrlToInfo { _controller = this._controllerName, _action = name };
                return result;
            }
        }

        string _controller;
        string _action;
        object[] _parameters;

        public UrlToInfo()
        {
        }

        public UrlToInfo(string controller)
        {
            this._controller = controller;
        }

        public UrlToInfo Action(string name)
        {
            this._action = name;
            return this;
        }

        public UrlToInfo Parameters(params object[] parameters)
        {
            this._parameters = parameters;
            return this;
        }

        public static implicit operator string(UrlToInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            return GenerateUrl(_controller, _action, _parameters);
        }

        public static string GenerateUrl(string controller, string action, params object[] parameters)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath.ToLower();
            var url = new StringBuilder();
            url.Append(appPath);
            if (!string.IsNullOrEmpty(appPath) && !appPath.EndsWith("/"))
            {
                url.Append("/");
            }
            url.Append(controller).Append("/");
            if (!string.IsNullOrEmpty(action))
            {
                url.Append(action).Append("/");
            }
            if (parameters != null)
            {
                foreach (var o in parameters)
                {
                    if (o != null)
                    {
                        AppendParameter(url, o);
                    }
                }
            }
            url.Length--;
            if (WebSettings.UsingAspxPostfix)
            {
                url.Append(".aspx");
            }
            return url.ToString();
        }

        private static void AppendParameter(StringBuilder url, object o)
        {
            if (o is IEnumerable && !(o is string))
            {
                foreach (var obj in (IEnumerable)o)
                {
                    AppendParameter(url, obj);
                }
            }
            else
            {
                url.Append(HttpUtility.UrlEncode(o.ToString())).Append("/");
            }
        }
    }
}
