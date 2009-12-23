using System;
using System.Text;
using System.IO;
using Lephone.Util.Setting;

namespace Lephone.Util.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
		protected string LogFileName;

		public TextFileLogRecorder()
		{
		    string s = UtilSetting.LogFileName;
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

        public virtual void ProcessLog(LogType type, string source, string name, string message, Exception eException)
        {
            using (var sw = new StreamWriter(LogFileName, true, Encoding.Default))
            {
                sw.WriteLine("{0},{1},{2},{3},{4},{5}", type, source, name, message, eException, DateTime.Now);
            }
        }
    }
}
