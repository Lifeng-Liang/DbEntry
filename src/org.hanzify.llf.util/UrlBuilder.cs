
#region usings

using System;
using System.Text;
using System.Collections.Specialized;
using System.Web;

#endregion

namespace org.hanzify.llf.util
{
    public class UrlBuilder
    {
        private string _BaseUrl;
        private NameValueCollection _Params;

        public UrlBuilder(string BaseUrl)
        {
            _BaseUrl = BaseUrl;
            _Params = new NameValueCollection();
        }

        public void Add(string Key, string Value)
        {
            _Params.Add(Key, Value); 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(_BaseUrl);
            bool HasParam = (_BaseUrl.IndexOf("?") >= 0);
            sb.Append(HasParam ? "&" : "?");
            foreach (string Key in _Params.AllKeys)
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
