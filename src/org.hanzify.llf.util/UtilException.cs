
using System;
using System.Runtime.Serialization;

namespace Lephone.Util
{
    public class UtilException : LephoneException
    {
		public UtilException() { }
		public UtilException(string ErrorMessage) : base(ErrorMessage) { }
        public UtilException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected UtilException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public UtilException(string message, Exception innerException) : base(message, innerException) { }
    }
}
