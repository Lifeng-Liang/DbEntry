using System;
using Leafing.Core.Ioc;

namespace Leafing.Core.Logging
{
    [DependenceEntry]
	public interface ILogRecorder
	{
        void ProcessLog(SysLogType type, string name, string message, Exception exception);
	}
}
