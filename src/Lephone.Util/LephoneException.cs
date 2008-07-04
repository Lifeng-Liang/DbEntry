using System;
using System.Runtime.Serialization;

namespace Lephone.Util
{
    public class LephoneException : Exception
    {
		public LephoneException() { }
		public LephoneException(string ErrorMessage) : base(ErrorMessage) { }
        public LephoneException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected LephoneException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LephoneException(string message, Exception innerException) : base(message, innerException) { }
    }
}
