using System;
using System.Runtime.Serialization;

namespace Lephone.Core
{
    public class SettingException : UtilException
	{
		public SettingException() : base("Setting Error.") {}
		public SettingException(string errorMessage) : base(errorMessage) {}
        public SettingException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected SettingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public SettingException(string message, Exception innerException) : base(message, innerException) {}
	}
}
