using System;
using Lephone.Core.Setting;

namespace Lephone.Core.Logging
{
    /// <summary>
    /// the eException could be null
    /// </summary>
    public delegate void LogEventHandler(LogType type, string source, string name, string message, Exception eException);

    public class Logger : ILogDirect
	{
        public static readonly Logger SQL = new Logger("SQL");
        public static readonly Logger Default = new Logger("Default");
        public static readonly Logger System = new Logger("System");

        public event LogEventHandler LogEvent;
        private readonly string _name;

        #region Constructors

        public Logger()
        {
            this._name = "NULL";
        }

		public Logger(string settingName)
        {
            _name = settingName;
            ILogRecorder ilc = null;
            string s = ConfigHelper.DefaultSettings.GetValue(_name + "LogRecorder");
            if (s != "")
            {
                ilc = LogRecorderProvider.GetLogRecorder(s);
                if (ilc == null)
                {
                    throw new SettingException();
                }
            }
            Init(ilc);
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

        void ILogDirect.Log(LogType type, string name, string message)
        {
            ((ILogDirect)this).Log(type, SystemHelper.CallerFunctionName, name, message, null);
        }

        void ILogDirect.Log(LogType type, string source, string name, string message)
        {
            ((ILogDirect)this).Log(type, source, name, message, null);
        }

        void ILogDirect.Log(LogType type, string name, string message, Exception eException)
        {
            ((ILogDirect)this).Log(type, SystemHelper.CallerFunctionName, name, message, eException);
        }

        void ILogDirect.Log(LogType type, string source, string name, string message, Exception eException)
        {
            if (LogEvent != null)
            {
                LogEvent(type, source, name, message, eException);
            }
        }

        #endregion

        public void Debug(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Debug, "", _name, message.ToString());
        }

        public void Trace(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Trace, "", _name, message.ToString());
        }

        public void Info(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Info, "", _name, message.ToString());
        }

        public void Warn(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Warn(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Warn(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }

        public void Error(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Error(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Error(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }

        public void Fatal(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, _name, message.ToString());
        }

        public void Fatal(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, _name, ex.Message, ex);
        }

        public void Fatal(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, _name, message.ToString(), ex);
        }
    }
}
