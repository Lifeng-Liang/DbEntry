using System;
using System.Runtime.Serialization;

namespace Lephone.Util
{
    public class SettingException : UtilException
	{
		public SettingException() : base("Setting Error.") {}
		public SettingException(string ErrorMessage) : base(ErrorMessage) {}
        public SettingException(string msgFormat, params object[] os) : base(String.Format(msgFormat, os)) { }
		protected SettingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public SettingException(string message, Exception innerException) : base(message, innerException) {}
	}
}
