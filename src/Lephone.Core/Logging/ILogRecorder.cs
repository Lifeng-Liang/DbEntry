using System;

namespace Lephone.Core.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(SysLogType type, string source, string name, string message, Exception exception);
	}
}
