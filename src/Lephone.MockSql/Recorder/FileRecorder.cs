using System.IO;
using Lephone.Util;
using Lephone.Util.Setting;

namespace Lephone.MockSql.Recorder
{
    public class FileRecorder : IRecorder
    {
        private readonly string FileName;

        public FileRecorder()
        {
            FileName = ConfigHelper.AppSettings.GetValue("RecorderFileName");
        }

        public void Write(string Msg, params object[] os)
        {
            using ( StreamWriter sw = new StreamWriter(FileName, true, EncodingEx.Default) )
            {
                sw.WriteLine(Msg, os);
            }
        }
    }
}
