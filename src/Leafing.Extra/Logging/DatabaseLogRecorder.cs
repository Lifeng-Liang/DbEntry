using System;
using Leafing.Core.Ioc;
using Leafing.Core.Logging;
using Leafing.Data;

namespace Leafing.Extra.Logging
{
    [Implementation("Database")]
    public class DatabaseLogRecorder : ILogRecorder
    {
        public void ProcessLog(LogLevel type, string name, string message, Exception exception)
        {
            var li = new LeafingLog(type, name, message, exception);
            try
            {
				DbEntry.NewTransaction(() => DbEntry.Save(li));
            }
            catch (Exception ex)
            {
                string msg = (exception == null) ? message : message + "\n" + exception;
                if(Logger.System.LogRecorders != null)
                {
                    foreach(var recorder in Logger.System.LogRecorders)
                    {
                        recorder.ProcessLog(type, name, msg, ex);
                    }
                }
            }
        }
    }
}
