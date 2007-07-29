
#region usings

using System;
using org.hanzify.llf.util.Logging;

#endregion

namespace org.hanzify.llf.Data.Logging
{
	public static class Log2db
	{
        public static Logger Instance
        {
            get { return Logger.Default; }
        }

        static Log2db()
		{
            DatabaseLogRecorder r = new DatabaseLogRecorder();
            // Logger.SQL.LogEvent += new LogEventHandler(r.ProcessLog);
            Logger.Default.LogEvent += new LogEventHandler(r.ProcessLog);
        }
    }
}
