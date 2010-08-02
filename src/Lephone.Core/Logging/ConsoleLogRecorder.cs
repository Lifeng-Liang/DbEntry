using System;

namespace Lephone.Core.Logging
{
    public class ConsoleLogRecorder : ILogRecorder
	{
        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            Console.WriteLine("{0},{1},{2},{3},{4}", type, source, name, message, exception);
        }
    }
}
