using Leafing.Core.Logging;

namespace Leafing.Extra.Logging
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
            Logger.SQL.LogRecorders.Add(r);
            Logger.Default.LogRecorders.Add(r);
        }
    }
}
