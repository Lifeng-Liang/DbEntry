using System;
using Lephone.Core.Logging;
using Lephone.Data;

namespace Lephone.Extra.Logging
{
    public class DatabaseLogRecorder : ILogRecorder
    {
        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            var li = new LephoneLog(type, source, name, message, exception);
            try
            {
                DbEntry.Save(li);
            }
            catch (Exception ex)
            {
                string msg = (exception == null) ? message : message + "\n" + exception;
                ((ILogDirect)Logger.System).Log(type, source, name, msg, ex);
            }
        }
    }
}
