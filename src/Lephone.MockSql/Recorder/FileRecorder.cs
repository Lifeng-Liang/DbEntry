using System.IO;
using Lephone.Core;
using Lephone.Core.Setting;

namespace Lephone.MockSql.Recorder
{
    public class FileRecorder : IRecorder
    {
        private readonly string _fileName;

        public FileRecorder()
        {
            _fileName = ConfigHelper.AppSettings.GetValue("RecorderFileName");
        }

        public void Write(string msg, params object[] os)
        {
            using (var sw = new StreamWriter(_fileName, true, EncodingEx.Default))
            {
                sw.WriteLine(msg, os);
            }
        }
    }
}
