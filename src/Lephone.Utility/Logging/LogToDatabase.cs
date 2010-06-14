using Lephone.Core.Logging;

namespace Lephone.Utility.Logging
{
	public static class LogToDatabase
	{
        public static Logger Instance
        {
            get { return Logger.Default; }
        }

        static LogToDatabase()
		{
            var r = new DatabaseLogRecorder();
            Logger.SQL.LogEvent += r.ProcessLog;
            Logger.Default.LogEvent += r.ProcessLog;
        }
    }
}
