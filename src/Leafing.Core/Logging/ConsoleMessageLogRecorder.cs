using System;
using Leafing.Core.Ioc;

namespace Leafing.Core.Logging
{
    [Implementation("ConsoleMessage")]
    public class ConsoleMessageLogRecorder : ILogRecorder
    {
        private static long _count;

        public static long Count
        {
            get { return _count; }
        }

        public void ProcessLog(LogLevel type, string name, string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            _count++;
        }
    }
}
