using System;
using System.Runtime.Serialization;
using Leafing.Core;

namespace Leafing.Data
{
    [Serializable]
	public class DataException : LeafingException
	{
		public DataException() : base("DataBase Error.") {}
		public DataException(string errorMessage) : base(errorMessage) {}
        public DataException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected DataException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public DataException(string message, Exception innerException) : base(message, innerException) {}
	}
}
