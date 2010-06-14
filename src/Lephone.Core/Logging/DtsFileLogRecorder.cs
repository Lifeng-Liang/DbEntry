using System;
using System.IO;

namespace Lephone.Core.Logging
{
    public class DtsFileLogRecorder : TextFileLogRecorder
    {
        public DtsFileLogRecorder() { }

        public DtsFileLogRecorder(string logFileName) : base(logFileName) { }

        protected override void WriteLog(StreamWriter sw, LogType type, string source, string name, string message, Exception eException)
        {
            sw.WriteLine("{0},{1},{2},{3},{4},{5}", GetString4Dts(type), GetString4Dts(source),
                GetString4Dts(name), GetString4Dts(message), GetString4Dts(eException), GetString4Dts(DateTime.Now));
        }

        private static string GetString4Dts(object o)
        {
            if(o != null)
            {
                var s = o.ToString();
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            }
            return "null";
        }
    }
}
