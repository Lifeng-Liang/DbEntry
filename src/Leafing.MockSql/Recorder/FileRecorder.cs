using System.IO;
using System.Text;
using Leafing.Core.Setting;

namespace Leafing.MockSql.Recorder
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
            using (var sw = new StreamWriter(_fileName, true, Encoding.Default))
            {
                sw.WriteLine(msg, os);
            }
        }
    }
}
