using System;
using System.Collections.Generic;
using Leafing.Core.Setting;

namespace Leafing.Core.Logging
{
    public class Logger
	{
        // ReSharper disable InconsistentNaming
        public static readonly Logger SQL = new Logger("SQL");
        public static readonly Logger Default = new Logger("Default");
        public static readonly Logger System = new Logger("System");
        // ReSharper restore InconsistentNaming

        public readonly List<ILogRecorder> LogRecorders;
        private readonly string _name;

        #region Constructors

        public Logger()
        {
            this._name = "NULL";
        }

        public Logger(params ILogRecorder[] recorders)
        {
            LogRecorders = new List<ILogRecorder>();
            LogRecorders.AddRange(recorders);
        }

        public Logger(string settingName)
        {
            _name = settingName;
		    LogRecorders = GetLogRecorders();
        }

        private List<ILogRecorder> GetLogRecorders()
        {
            string s = ConfigHelper.LeafingSettings.GetValue(_name + "LogRecorder");
            if (s != "")
            {
                var list = new List<ILogRecorder>();
                var ss = s.Split('|');
                foreach (var s1 in ss)
                {
                    var s2 = s1.Trim();
                    if (s2 != "")
                    {
                        var ilc = LogRecorderProvider.GetLogRecorder(s2);
                        list.Add(ilc);
                    }
                }
                return list;
            }
            return null;
        }

        #endregion

        protected void Log(LogLevel level, string name, string message, Exception exception)
        {
			if (level == LogLevel.All || level == LogLevel.Off) {
				throw new CoreException("LogLevel can not be ALL or OFF for logging.");
			}
			if(LogRecorders != null && level <= CoreSettings.LogLevel)
            {
                foreach (var recorder in LogRecorders)
                {
                    recorder.ProcessLog(level, name, message, exception);
                }
            }
        }

        public void Debug(object message)
        {
            Debug(() => message);
        }

        public void Debug(Func<object> callback)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Debug, _name, callback().ToString(), null);
            }
        }

        public void Trace(object message)
        {
            Trace(() => message);
        }

        public void Trace(Func<object> callback)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Trace, _name, callback().ToString(), null);
            }
        }

        public void Info(object message)
        {
            Info(() => message);
        }

        public void Info(Func<object> callback)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Info, _name, callback().ToString(), null);
            }
        }

        public void Warn(object message)
        {
            Warn(() => message, null);
        }

        public void Warn(Func<object> callback)
        {
            Warn(callback, null);
        }

        public void Warn(Exception ex)
        {
            Warn(() => ex.Message, ex);
        }

        public void Warn(object message, Exception ex)
        {
            Warn(() => message, ex);
        }

        public void Warn(Func<object> callback, Exception ex)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Warn, _name, callback().ToString(), ex);
            }
        }

        public void Error(object message)
        {
            Error(() => message, null);
        }

        public void Error(Exception ex)
        {
            Error(() => ex.Message, ex);
        }

        public void Error(object message, Exception ex)
        {
            Error(() => message, ex);
        }

        public void Error(Func<object> callback, Exception ex)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Error, _name, callback().ToString(), ex);
            }
        }

        public void Fatal(object message)
        {
            Fatal(() => message, null);
        }

        public void Fatal(Exception ex)
        {
            Fatal(() => ex.Message, ex);
        }

        public void Fatal(object message, Exception ex)
        {
            Fatal(() => message, ex);
        }

        public void Fatal(Func<object> callback, Exception ex)
        {
            if (LogRecorders != null)
            {
                Log(LogLevel.Fatal, _name, callback().ToString(), ex);
            }
        }
    }
}
