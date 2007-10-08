
using System;
using System.Runtime.Serialization;

namespace Lephone.Util
{
    public class LephoneException : Exception
    {
		public LephoneException() : base() {}
		public LephoneException(string ErrorMessage) : base(ErrorMessage) {}
		protected LephoneException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public LephoneException(string message, Exception innerException) : base(message, innerException) { }
    }
}
