using System;
using System.Runtime.Serialization;
using Lephone.Util;

namespace Lephone.Data
{
	public class DataException : LephoneException
	{
		public DataException() : base("DataBase Error.") {}
		public DataException(string ErrorMessage) : base(ErrorMessage) {}
        public DataException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected DataException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public DataException(string message, Exception innerException) : base(message, innerException) {}
	}
}
