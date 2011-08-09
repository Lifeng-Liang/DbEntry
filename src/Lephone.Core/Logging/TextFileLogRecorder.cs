using System;
using System.Text;
using System.IO;
using Lephone.Core.Setting;

namespace Lephone.Core.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
        protected object SyncRoot = new object();

	    private string _logFileName;
	    private readonly string _logFileTemplate;
	    private Date _lastDate = Date.MinValue;

        protected string LogFileName
        {
            set { _logFileName = value; }
            get
            {
                if (_lastDate != Date.Now)
                {
                    _logFileName = string.Format(_logFileTemplate, SystemHelper.BaseDirectory,
                                                 SystemHelper.ExeFileName, SystemHelper.GetDateTimeString());
                    _lastDate = Date.Now;
                }
                return _logFileName;
            }
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
            // ReSharper disable RedundantToStringCall
            LogFileName.ToString(); // force to read LogFileName
            // ReSharper restore RedundantToStringCall
        }

        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            lock (SyncRoot)
            {
                using (var sw = new StreamWriter(LogFileName, true, Encoding.Default))
                {
                	WriteLog(sw, type, source, name, message, exception);
                }
            }
        }

        protected virtual void WriteLog(StreamWriter sw, SysLogType type, string source, string name, string message, Exception exception)
        {
            sw.WriteLine("{0},{1},{2},{3},{4},{5}", type, source, name, message, exception, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
