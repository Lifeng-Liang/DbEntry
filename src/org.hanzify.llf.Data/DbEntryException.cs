
#region usings

using System;
using System.Data.Common;
using System.Runtime.Serialization;

#endregion

namespace Lephone.Data
{
	public class DbEntryException : DbException
	{
		public DbEntryException() : base("DataBase Error.") {}
		public DbEntryException(string ErrorMessage) : base(ErrorMessage) {}
		protected DbEntryException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public DbEntryException(string message, Exception innerException) : base(message, innerException) {}
	}
}
