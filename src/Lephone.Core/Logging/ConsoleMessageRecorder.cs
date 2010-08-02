using System;

namespace Lephone.Core.Logging
{
    public class ConsoleMessageRecorder : ILogRecorder
    {
        private static long _count;

        public static long Count
        {
            get { return _count; }
        }

        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            _count++;
        }
    }
}
