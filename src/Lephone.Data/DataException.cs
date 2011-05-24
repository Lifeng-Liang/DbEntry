﻿using System;
using System.Runtime.Serialization;
using Lephone.Core;

namespace Lephone.Data
{
    [Serializable]
	public class DataException : LephoneException
	{
		public DataException() : base("DataBase Error.") {}
		public DataException(string errorMessage) : base(errorMessage) {}
        public DataException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected DataException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public DataException(string message, Exception innerException) : base(message, innerException) {}
	}
}
