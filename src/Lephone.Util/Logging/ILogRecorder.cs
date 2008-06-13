using System;

namespace Lephone.Util.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(LogType type, string source, string name, string message, Exception eException);
	}
}
