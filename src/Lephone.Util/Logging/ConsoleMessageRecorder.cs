using System;

namespace Lephone.Util.Logging
{
    public class ConsoleMessageRecorder : ILogRecorder
    {
        public void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}
