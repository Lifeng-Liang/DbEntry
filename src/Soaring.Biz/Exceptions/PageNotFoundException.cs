using System;
using System.Runtime.Serialization;
using Lephone.Web;

namespace Soaring.Biz.Exceptions
{
    public class PageNotFoundException : WebException
    {
        public PageNotFoundException() : base("Setting Error.") { }
        public PageNotFoundException(string errorMessage) : base(errorMessage) { }
        public PageNotFoundException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected PageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public PageNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
