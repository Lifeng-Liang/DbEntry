
using System;
using System.Runtime.Serialization;
using Lephone.Util;

namespace Lephone.Web
{
    public class WebException : LephoneException
    {
		public WebException() : base("Setting Error.") { }
		public WebException(string ErrorMessage) : base(ErrorMessage) { }
        public WebException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected WebException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public WebException(string message, Exception innerException) : base(message, innerException) { }
    }
}
