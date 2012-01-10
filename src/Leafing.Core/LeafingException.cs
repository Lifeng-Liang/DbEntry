using System;
using System.Runtime.Serialization;

namespace Leafing.Core
{
    [Serializable]
    public class LeafingException : Exception
    {
		public LeafingException() { }
		public LeafingException(string errorMessage) : base(errorMessage) { }
        public LeafingException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected LeafingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LeafingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
