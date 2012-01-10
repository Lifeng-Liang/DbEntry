using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Leafing.MockSql
{
    public class MockDbException : DbException
    {
        public MockDbException() : base("DataBase Error.") { }
        public MockDbException(string errorMessage) : base(errorMessage) { }
        protected MockDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public MockDbException(string message, Exception innerException) : base(message, innerException) { }
    }
}
