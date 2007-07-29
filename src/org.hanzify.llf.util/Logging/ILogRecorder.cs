
using System;

namespace org.hanzify.llf.util.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException);
	}
}
