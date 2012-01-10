using System;

namespace Leafing.Core.Logging
{
    public class ConsoleLogRecorder : ILogRecorder
	{
        public void ProcessLog(SysLogType type, string name, string message, Exception exception)
        {
            Console.WriteLine("{0},{1},{2},{3}", type, name, message, exception);
        }
    }
}
