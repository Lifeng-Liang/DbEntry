
#region usings

using System;
using System.Data.Common;
using System.Runtime.Serialization;

#endregion

namespace org.hanzify.llf.MockSql
{
    public class MockDbException : DbException
    {
        public MockDbException() : base("DataBase Error.") { }
        public MockDbException(string ErrorMessage) : base(ErrorMessage) { }
        protected MockDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public MockDbException(string message, Exception innerException) : base(message, innerException) { }
    }
}
