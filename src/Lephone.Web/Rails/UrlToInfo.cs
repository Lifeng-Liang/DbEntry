using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Lephone.Web.Rails
{
    public class UrlToInfo
    {
        protected string _controller;
        protected string _action;
        protected object[] _parameters;
        protected List<KeyValuePair<string, string>> _urlParam;

        public UrlToInfo()
        {
        }

        public UrlToInfo(string controller)
        {
            this._controller = controller;
        }

        public UrlToInfo Controller(string name)
        {
            this._controller = name;
            return this;
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

        public UrlToInfo UrlParam(string key, string value)
        {
            if (_urlParam == null)
            {
                _urlParam = new List<KeyValuePair<string, string>>();
            }
            _urlParam.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public static implicit operator string(UrlToInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            return GenerateUrl(_controller, _action, _parameters, _urlParam);
        }

        public static string GenerateUrl(string controller, string action, 
            object[] parameters, List<KeyValuePair<string, string>> urlParam)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath.ToLower();
            var url = new StringBuilder();
            url.Append(appPath);
            if (!string.IsNullOrEmpty(appPath) && !appPath.EndsWith("/"))
            {
                url.Append("/");
            }
            url.Append(controller.ToLower()).Append("/");
            if (!string.IsNullOrEmpty(action))
            {
                url.Append(action.ToLower()).Append("/");
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
            if (WebSettings.RailsPostfix != "")
            {
                url.Append(WebSettings.RailsPostfix);
            }
            var s =  url.ToString();
            if(urlParam != null)
            {
                var ub = new UrlBuilder(s, Encoding.UTF8);
                foreach (var kv in urlParam)
                {
                    ub.Add(kv.Key, kv.Value);
                }
                return ub.ToString();
            }
            return s;
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
