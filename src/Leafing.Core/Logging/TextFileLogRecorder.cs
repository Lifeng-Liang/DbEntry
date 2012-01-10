using System;
using System.Text;
using System.IO;
using Leafing.Core.Setting;

namespace Leafing.Core.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
        protected object SyncRoot = new object();

	    private string _logFileName;
	    private readonly string _logFileTemplate;
	    private Date _lastDate = Date.MinValue;

        protected string GetLogFileName()
        {
            if (_lastDate != Date.Now)
            {
                _logFileName = string.Format(_logFileTemplate, SystemHelper.BaseDirectory,
                                             SystemHelper.ExeFileName, SystemHelper.GetDateTimeString());
                _lastDate = Date.Now;
            }
            return _logFileName;
        }

		public TextFileLogRecorder()
		{
            _logFileTemplate = CoreSettings.LogFileName;
			Init();
		}

        public TextFileLogRecorder(string logFileName)
		{
            _logFileTemplate = logFileName;
            Init();
		}

		protected void Init()
		{
            if (_logFileTemplate == "")
			{
				throw new SettingException();
			}
		    GetLogFileName();
        }

        public void ProcessLog(SysLogType type, string name, string message, Exception exception)
        {
            lock (SyncRoot)
            {
                using (var sw = new StreamWriter(GetLogFileName(), true, Encoding.Default))
                {
                	WriteLog(sw, type, name, message, exception);
                }
            }
        }

        protected virtual void WriteLog(StreamWriter sw, SysLogType type, string name, string message, Exception exception)
        {
            sw.WriteLine("{0},{1},{2},{3},{4}", type, name, message, exception, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
