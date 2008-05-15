using System;

namespace Lephone.Util.Logging
{
    public class ConsoleLogRecorder : ILogRecorder
	{
        public void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            Console.WriteLine("{0},{1},{2},{3},{4}", Type, Source, Name, Message, eException);
        }
    }
}
