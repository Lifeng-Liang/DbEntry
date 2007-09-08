
#region usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Lephone.Util
{
	public class SettingException : Exception
	{
		public SettingException() : base("Setting Error.") {}
		public SettingException(string ErrorMessage) : base(ErrorMessage) {}
		protected SettingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public SettingException(string message, Exception innerException) : base(message, innerException) {}
	}
}
