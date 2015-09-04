using System;
using Leafing.Data.Definition;
using Leafing.Core.Logging;

namespace Leafing.Extra.Logging
{
	[DisableSqlLog]
    public class LeafingLog : DbObjectModel<LeafingLog>
	{
        public LogLevel Type { get; set; }
        public string Thread { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        [SpecialName] public DateTime CreatedOn { get; set; }

        public LeafingLog() {}

        public LeafingLog(LogLevel type, string name, string message, Exception exception)
		{
			this.Type = type;
			this.Thread = GetThreadInfo();
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
