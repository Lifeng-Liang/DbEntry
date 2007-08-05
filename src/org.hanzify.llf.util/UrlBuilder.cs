
#region usings

using System;
using System.Text;
using System.Collections.Generic;
using System.Web;

#endregion

namespace org.hanzify.llf.util
{
    public class UrlBuilder
    {
        private string _BaseUrl;
        private Dictionary<string, byte[]> _Params;
        private Encoding _Encoding;

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

        public void Add(string Key, string Value)
        {
            Add(Key, Value, _Encoding);
        }

        public void Add(string Key, string Value, Encoding encoding)
        {
            _Params.Add(Key, encoding.GetBytes(Value));
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
