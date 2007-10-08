
using System;
using System.Runtime.Serialization;

namespace Lephone.Util
{
    public class UtilException : LephoneException
    {
		public UtilException() : base() {}
		public UtilException(string ErrorMessage) : base(ErrorMessage) {}
		protected UtilException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public UtilException(string message, Exception innerException) : base(message, innerException) { }
    }
}
