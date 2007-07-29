
#region usings

using System;
using System.IO;

using org.hanzify.llf.util;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.MockSql.Recorder
{
    public class FileRecorder : IRecorder
    {
        private string FileName;

        public FileRecorder()
        {
            this.FileName = ConfigHelper.AppSettings.GetValue("RecorderFileName");
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
