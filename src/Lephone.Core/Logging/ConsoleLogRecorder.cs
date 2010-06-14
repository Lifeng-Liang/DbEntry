using System;

namespace Lephone.Core.Logging
{
    public class ConsoleLogRecorder : ILogRecorder
	{
        public void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            Console.WriteLine("{0},{1},{2},{3},{4}", type, source, name, message, eException);
        }
    }
}
