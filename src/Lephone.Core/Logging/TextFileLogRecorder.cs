using System;
using System.Text;
using System.IO;
using Lephone.Core.Setting;

namespace Lephone.Core.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
        protected object SyncRoot = new object();

		protected string LogFileName;

		public TextFileLogRecorder()
		{
		    string s = CoreSettings.LogFileName;
			Init(s);
		}

        public TextFileLogRecorder(string logFileName)
		{
			Init(logFileName);
		}

		protected void Init(string logFileName)
		{
			if ( logFileName == "" )
			{
				throw new SettingException();
			}
			this.LogFileName = string.Format(logFileName, SystemHelper.BaseDirectory,
                SystemHelper.ExeFileName, SystemHelper.GetDateTimeString());
		}

        public void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            lock (SyncRoot)
            {
                using (var sw = new StreamWriter(LogFileName, true, Encoding.Default))
                {
                	WriteLog(sw, type, source, name, message, eException);
                }
            }
        }

        protected virtual void WriteLog(StreamWriter sw, LogType type, string source, string name, string message, Exception eException)
        {
            sw.WriteLine("{0},{1},{2},{3},{4},{5}", type, source, name, message, eException, DateTime.Now);
        }
    }
}
