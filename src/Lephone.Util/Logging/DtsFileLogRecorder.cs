using System;
using System.IO;
using System.Text;

namespace Lephone.Util.Logging
{
    public class DtsFileLogRecorder : TextFileLogRecorder
    {
        public DtsFileLogRecorder() { }

        public DtsFileLogRecorder(string LogFileName) : base(LogFileName) { }

        public override void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            using (StreamWriter sw = new StreamWriter(LogFileName, true, Encoding.Default))
            {
                sw.WriteLine("{0},{1},{2},{3},{4}", type, GetString4Dts(source),
                    GetString4Dts(name), GetString4Dts(message), GetString4Dts(eException.ToString()));
            }
        }

        private string GetString4Dts(object o)
        {
            if (o is string)
            {
                return "\"" + ((string)o).Replace("\"", "\"\"") + "\"";
            }
            return o.ToString();
        }
    }
}
