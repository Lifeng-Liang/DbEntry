using Lephone.Util.Logging;

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
            Logger.SQL.LogEvent += r.ProcessLog;
            Logger.Default.LogEvent += r.ProcessLog;
        }
    }
}
