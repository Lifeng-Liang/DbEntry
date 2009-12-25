using System;
using System.IO;
using System.Text;

namespace Lephone.Util.Logging
{
    public class DtsFileLogRecorder : TextFileLogRecorder
    {
        public DtsFileLogRecorder() { }

        public DtsFileLogRecorder(string logFileName) : base(logFileName) { }

        public override void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            using (var sw = new StreamWriter(LogFileName, true, Encoding.Default))
            {
                sw.WriteLine("{0},{1},{2},{3},{4},{5}", GetString4Dts(type), GetString4Dts(source),
                    GetString4Dts(name), GetString4Dts(message), GetString4Dts(eException), GetString4Dts(DateTime.Now));
            }
        }

        private string GetString4Dts(object o)
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
