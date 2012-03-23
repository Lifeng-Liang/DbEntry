using System;
using System.IO;
using Leafing.Core.Ioc;

namespace Leafing.Core.Logging
{
    [Implementation("TextFileMessage")]
    public class TextFileMessageLogRecorder : TextFileLogRecorder
    {
        protected override void WriteLog(StreamWriter sw, SysLogType type, string name, string message, Exception exception)
        {
            sw.WriteLine(message);
        }
    }
}
