using System;
using System.Runtime.Serialization;
using Lephone.Core;

namespace Lephone.Data.Model.Linq
{
    public class LinqException : LephoneException
    {
        public LinqException() { }
        public LinqException(string errorMessage) : base(errorMessage) { }
        public LinqException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected LinqException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public LinqException(string message, Exception innerException) : base(message, innerException) { }
    }
}