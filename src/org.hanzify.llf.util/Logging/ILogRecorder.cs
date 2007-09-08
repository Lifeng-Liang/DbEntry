
using System;

namespace Lephone.Util.Logging
{
	public interface ILogRecorder
	{
        void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException);
	}
}
