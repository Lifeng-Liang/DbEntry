
using System;
using System.Runtime.Serialization;
using Lephone.Util;

namespace Lephone.Linq
{
    public class LinqException : LephoneException
    {
        public LinqException() : base() { }
        public LinqException(string ErrorMessage) : base(ErrorMessage) { }
        protected LinqException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LinqException(string message, Exception innerException) : base(message, innerException) { }
    }
}
