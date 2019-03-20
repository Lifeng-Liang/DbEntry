using System.IO;
using System.Text;
using Leafing.Core.Setting;

namespace Leafing.MockSql.Recorder {
    public class FileRecorder : IRecorder {
        private readonly string _fileName;

        public FileRecorder() {
            _fileName = ConfigReader.Config.AppSettings.GetValue("RecorderFileName");
        }

        public void Write(string msg, params object[] os) {
            using (var sw = new StreamWriter(_fileName, true, Encoding.UTF8)) {
                sw.WriteLine(msg, os);
            }
        }
    }
}