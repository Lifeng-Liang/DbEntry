using System;
using System.IO;

namespace Lephone.Core.Logging
{
    public class TextFileMessageLogRecorder : TextFileLogRecorder
    {
        public TextFileMessageLogRecorder() { }

        public TextFileMessageLogRecorder(string logFileName) : base(logFileName) { }

        protected override void WriteLog(StreamWriter sw, SysLogType type, string source, string name, string message, Exception exception)
        {
            sw.WriteLine(message);
        }
    }
}
