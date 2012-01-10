using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Leafing.Web.Mvc.Core
{
    public class UrlToInfo
    {
        protected string TheController;
        protected string TheAction;
        protected object[] TheParameters;
        protected List<KeyValuePair<string, string>> TheUrlParam;

        public UrlToInfo()
        {
        }

        public UrlToInfo(string controller)
        {
            this.TheController = controller;
        }

        public UrlToInfo Controller(string name)
        {
            this.TheController = name;
            return this;
        }

        public UrlToInfo Action(string name)
        {
            this.TheAction = name;
            return this;
        }

        public UrlToInfo Parameters(params object[] parameters)
        {
            this.TheParameters = parameters;
            return this;
        }

        public UrlToInfo UrlParam(string key, string value)
        {
            if (TheUrlParam == null)
            {
                TheUrlParam = new List<KeyValuePair<string, string>>();
            }
            TheUrlParam.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public static implicit operator string(UrlToInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            return GenerateUrl(TheController, TheAction, TheParameters, TheUrlParam);
        }

        public static string GenerateUrl(string controller, string action, 
                                         object[] parameters, List<KeyValuePair<string, string>> urlParam)
        {
            string appPath = HttpContextHandler.Instance.ApplicationPath.ToLower();
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
            if (WebSettings.MvcPostfix != "")
            {
                url.Append(WebSettings.MvcPostfix);
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


