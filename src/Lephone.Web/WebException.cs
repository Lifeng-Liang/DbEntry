using System;
using System.Runtime.Serialization;
using Lephone.Core;

namespace Lephone.Web
{
    [Serializable]
    public class WebException : LephoneException
    {
		public WebException() : base("Setting Error.") { }
		public WebException(string errorMessage) : base(errorMessage) { }
        public WebException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected WebException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public WebException(string message, Exception innerException) : base(message, innerException) { }
    }
}
