using System;
using Lephone.Data.Definition;
using Lephone.Core.Logging;

namespace Lephone.Utility.Logging
{
	[DisableSqlLog]
	public class LephoneLog : DbObject
	{
		public LogType Type;
		public string Thread;
		public string Source;
		public string Name;
		public string Message;
		public string Exception;
        [SpecialName] public DateTime CreatedOn;

        public LephoneLog() {}

        public LephoneLog(LogType type, string source, string name, string message, Exception exception)
		{
			this.Type = type;
			this.Thread = GetThreadInfo();
			this.Source = source;
			this.Name = name;
			this.Message = message;
            this.Exception = (exception == null) ? "" : exception.ToString();
		}

		private static string GetThreadInfo()
		{
			return GetThreadInfo(System.Threading.Thread.CurrentThread);
		}

		private static string GetThreadInfo(System.Threading.Thread t)
		{
			return string.Format("<{0}>{1},{2},{3},{4}",
				t.Name,
				t.GetApartmentState(),
				t.Priority,
				t.IsThreadPoolThread,
				t.IsBackground
				);
		}
	}
}
