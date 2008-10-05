using System;

namespace Lephone.Util.Logging
{
    public class ConsoleMessageRecorder : ILogRecorder
    {
        private static long _count;

        public static long Count
        {
            get { return _count; }
        }

        public void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            _count++;
        }
    }
}
