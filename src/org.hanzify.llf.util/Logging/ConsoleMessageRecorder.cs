
using System;

namespace Lephone.Util.Logging
{
    public class ConsoleMessageRecorder : ILogRecorder
    {
        public void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            Console.WriteLine(Message);
            Console.WriteLine();
        }
    }
}
