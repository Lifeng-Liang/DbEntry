
#region usings

using System;
using Lephone.Util.Logging;

#endregion

namespace Lephone.Data.Logging
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
            Logger.SQL.LogEvent += new LogEventHandler(r.ProcessLog);
            Logger.Default.LogEvent += new LogEventHandler(r.ProcessLog);
        }
    }
}
