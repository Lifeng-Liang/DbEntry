using System;
using System.Runtime.Serialization;

namespace Leafing.Core.Ioc
{
    [Serializable]
    public class IocException : CoreException
    {
		public IocException() { }
		public IocException(string errorMessage) : base(errorMessage) { }
        public IocException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected IocException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public IocException(string message, Exception innerException) : base(message, innerException) { }
    }
}
