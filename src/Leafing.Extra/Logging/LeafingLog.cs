using System;
using Leafing.Data.Definition;
using Leafing.Core.Logging;

namespace Leafing.Extra.Logging
{
	[DisableSqlLog]
	public class LeafingLog : DbObject
	{
		public SysLogType Type;
		public string Thread;
		public string Name;
		public string Message;
		public string Exception;
        [SpecialName] public DateTime CreatedOn;

        public LeafingLog() {}

        public LeafingLog(SysLogType type, string name, string message, Exception exception)
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
