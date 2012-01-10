using System;

namespace Leafing.Core.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(SysLogType type, string name, string message, Exception exception);
	}
}
