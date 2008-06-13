using System.Text;
using System.Collections.Generic;
using System.Web;

namespace Lephone.Web
{
    public class UrlBuilder
    {
        private readonly string _BaseUrl;
        private readonly Dictionary<string, byte[]> _Params;
        private readonly Encoding _Encoding;

        public UrlBuilder(string BaseUrl)
            : this(BaseUrl, Encoding.Default)
        {
        }

        public UrlBuilder(string BaseUrl, Encoding defaultEncoding)
        {
            _BaseUrl = BaseUrl;
            _Params = new Dictionary<string, byte[]>();
            _Encoding = defaultEncoding;
        }

        public UrlBuilder Add(string Key, string Value)
        {
            Add(Key, Value, _Encoding);
            return this;
        }

        public UrlBuilder Add(string Key, string Value, Encoding encoding)
        {
            _Params.Add(Key, encoding.GetBytes(Value));
            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(_BaseUrl);
            bool HasParam = (_BaseUrl.IndexOf("?") >= 0);
            sb.Append(HasParam ? "&" : "?");
            foreach (string Key in _Params.Keys)
            {
                sb.Append(HttpUtility.UrlEncode(Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(_Params[Key]));
                sb.Append("&");
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
