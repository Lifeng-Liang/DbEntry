using System;
using Lephone.Core.Setting;

namespace Lephone.Core.Logging
{
    /// <summary>
    /// the exception could be null
    /// </summary>
    public delegate void LogEventHandler(SysLogType type, string source, string name, string message, Exception exception);

    public class Logger : ILogDirect
	{
        public static readonly Logger SQL = new Logger("SQL");
        public static readonly Logger Default = new Logger("Default");
        public static readonly Logger System = new Logger("System");

        public LogEventHandler LogEvent;
        private readonly string _name;

        #region Constructors

        public Logger()
        {
            this._name = "NULL";
        }

		public Logger(string settingName)
        {
            _name = settingName;
		    string s = ConfigHelper.DefaultSettings.GetValue(_name + "LogRecorder");
            if (s != "")
            {
                var ss = s.Split('#');
                foreach(var s1 in ss)
                {
                    var s2 = s1.Trim();
                    if(s2 != "")
                    {
                        if(s2.StartsWith("@"))
                        {
                            s2 = string.Format("Lephone.Core.Logging.{0}LogRecorder, Lephone.Core", s2.Substring(1));
                        }
                        var ilc = LogRecorderProvider.GetLogRecorder(s2);
                        Init(ilc);
                    }
                }
            }
        }

		public Logger(ILogRecorder ilc)
		{
            Init(ilc);
        }

        private void Init(ILogRecorder ilc)
        {
            if (ilc != null)
            {
                LogEvent += ilc.ProcessLog;
            }
        }

        #endregion

        #region ILogDirect

        void ILogDirect.Log(SysLogType type, string name, string message)
        {
            ((ILogDirect)this).Log(type, SystemHelper.CallerFunctionName, name, message, null);
        }

        void ILogDirect.Log(SysLogType type, string source, string name, string message)
        {
            ((ILogDirect)this).Log(type, source, name, message, null);
        }

        void ILogDirect.Log(SysLogType type, string name, string message, Exception exception)
        {
            ((ILogDirect)this).Log(type, SystemHelper.CallerFunctionName, name, message, exception);
        }

        void ILogDirect.Log(SysLogType type, string source, string name, string message, Exception exception)
        {
            if (LogEvent != null)
            {
                LogEvent(type, source, name, message, exception);
            }
        }

        #endregion

        public void Debug(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Debug, "", _name, message.ToString());
        }

        public void Trace(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Trace, "", _name, message.ToString());
        }

        public void Info(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Info, "", _name, message.ToString());
        }

        public void Warn(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Warn, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Warn(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Warn, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Warn(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Warn, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }

        public void Error(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Error, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Error(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Error, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Error(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Error, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }

        public void Fatal(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Fatal, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Fatal(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Fatal, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Fatal(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(SysLogType.Fatal, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }
    }
}
