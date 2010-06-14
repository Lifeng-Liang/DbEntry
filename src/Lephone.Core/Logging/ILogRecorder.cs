using System;

namespace Lephone.Core.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(LogType type, string source, string name, string message, Exception eException);
	}
}
