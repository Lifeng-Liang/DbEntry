
#region usings

using System;
using System.Text;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.util.Logging
{
    /// <summary>
    /// the eException could be null
    /// </summary>
    public delegate void LogEventHandler(LogType Type, string Source, string Name, string Message, Exception eException);

    public class Logger : ILogDirect
	{
        public static readonly Logger SQL = new Logger("SQL");
        public static readonly Logger Default = new Logger("Default");
        public static readonly Logger System = new Logger("System");

        public event LogEventHandler LogEvent;
        private string Name;

        #region Constructors

        public Logger()
        {
            this.Name = "NULL";
        }

		public Logger(string SettingName)
        {
            Name = SettingName;
            ILogRecorder ilc = null;
            string s = ConfigHelper.DefaultSettings.GetValue(Name + "LogRecorder", "");
            if (s != "")
            {
                ilc = (ILogRecorder)ClassHelper.CreateInstance(s);
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
                LogEvent += new LogEventHandler(ilc.ProcessLog);
            }
        }

        #endregion

        #region ILogDirect

        void ILogDirect.Log(LogType Type, string Name, string Message)
        {
            ((ILogDirect)this).Log(Type, SystemHelper.CallerFunctionName, Name, Message, null);
        }

        void ILogDirect.Log(LogType Type, string Source, string Name, string Message)
        {
            ((ILogDirect)this).Log(Type, Source, Name, Message, null);
        }

        void ILogDirect.Log(LogType Type, string Name, string Message, Exception eException)
        {
            ((ILogDirect)this).Log(Type, SystemHelper.CallerFunctionName, Name, Message, eException);
        }

        void ILogDirect.Log(LogType Type, string Source, string Name, string Message, Exception eException)
        {
            if (LogEvent != null)
            {
                LogEvent(Type, Source, Name, Message, eException);
            }
        }

        #endregion

        public void Debug(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Debug, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Trace(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Trace, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Info(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Info, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Warn(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Warn(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, Name, ex.Message, ex);
        }

        public void Warn(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Warn, SystemHelper.CallerFunctionName, Name, message.ToString(), ex);
        }

        public void Error(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Error(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, Name, ex.Message, ex);
        }

        public void Error(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Error, SystemHelper.CallerFunctionName, Name, message.ToString(), ex);
        }

        public void Fatal(object message)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, Name, message.ToString());
        }

        public void Fatal(Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, Name, ex.Message, ex);
        }

        public void Fatal(object message, Exception ex)
        {
            if (LogEvent != null)
                ((ILogDirect)this).Log(LogType.Fatal, SystemHelper.CallerFunctionName, Name, message.ToString(), ex);
        }
    }
}
