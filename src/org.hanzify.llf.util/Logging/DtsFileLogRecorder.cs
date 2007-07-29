
#region usings

using System;
using System.IO;
using System.Text;

#endregion

namespace org.hanzify.llf.util.Logging
{
    public class DtsFileLogRecorder : TextFileLogRecorder
    {
        public DtsFileLogRecorder() : base() { }

        public DtsFileLogRecorder(string LogFileName) : base(LogFileName) { }

        public override void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            using (StreamWriter sw = new StreamWriter(LogFileName, true, Encoding.Default))
            {
                sw.WriteLine("{0},{1},{2},{3},{4}", Type, GetString4Dts(Source),
                    GetString4Dts(Name), GetString4Dts(Message), GetString4Dts(eException.ToString()));
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
