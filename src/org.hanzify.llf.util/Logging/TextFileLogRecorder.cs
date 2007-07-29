
#region usings

using System;
using System.Text;
using System.IO;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.util.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
		protected string LogFileName;

		public TextFileLogRecorder()
		{
            string s = ConfigHelper.AppSettings.GetValue("LogFileName", "{0}.{1}.log");
			Init(s);
		}

        public TextFileLogRecorder(string LogFileName)
		{
			Init(LogFileName);
		}

		protected void Init(string LogFileName)
		{
			if ( LogFileName == "" )
			{
				throw new SettingException();
			}
			this.LogFileName = string.Format(LogFileName, SystemHelper.ExeFileName, SystemHelper.GetDateTimeString());
		}

        public virtual void ProcessLog(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            using (StreamWriter sw = new StreamWriter(LogFileName, true, Encoding.Default))
            {
                sw.WriteLine("{0},{1},{2},{3},{4}", Type, Source, Name, Message, eException);
            }
        }
    }
}
