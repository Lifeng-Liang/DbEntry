using System.Text;
using System.Collections.Generic;
using System.Web;

namespace Leafing.Web
{
    public class UrlBuilder
    {
        private readonly string _baseUrl;
        private readonly Dictionary<string, byte[]> _params;
        private readonly Encoding _encoding;

        public UrlBuilder(string baseUrl)
            : this(baseUrl, Encoding.UTF8)
        {
        }

        public UrlBuilder(string baseUrl, Encoding defaultEncoding)
        {
            _baseUrl = baseUrl;
            _params = new Dictionary<string, byte[]>();
            _encoding = defaultEncoding;
        }

        public UrlBuilder Add(string key, string value)
        {
            Add(key, value, _encoding);
            return this;
        }

        public UrlBuilder Add(string key, string value, Encoding encoding)
        {
            if(!string.IsNullOrEmpty(value))
            {
                _params.Add(key, encoding.GetBytes(value));
            }
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(_baseUrl);
            bool hasParam = (_baseUrl.IndexOf("?") >= 0);
            sb.Append(hasParam ? "&" : "?");
            foreach (string key in _params.Keys)
            {
                sb.Append(HttpUtility.UrlEncode(key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(_params[key]));
                sb.Append("&");
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
