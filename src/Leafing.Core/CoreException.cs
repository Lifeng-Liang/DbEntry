using System;
using System.Runtime.Serialization;

namespace Leafing.Core {
    [Serializable]
    public class CoreException : LeafingException {
        public CoreException() { }
        public CoreException(string errorMessage) : base(errorMessage) { }
        public CoreException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
        protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public CoreException(string message, Exception innerException) : base(message, innerException) { }
    }
}
