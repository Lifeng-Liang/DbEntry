using System;
using Lephone.Data.Definition;
using Lephone.Util.Logging;

namespace Lephone.Data.Logging
{
	[DisableSqlLog]
	public class LephoneLog : DbObject
	{
		public LogType Type;
		public string Thread;
		public string Source;
		public string Name;
		public string Message;
		[DbColumn("Exception")] public string eException;
        [SpecialName] public DateTime CreatedOn;

        public LephoneLog(LogType Type, string Source, string Name, string Message, Exception eException)
		{
			this.Type = Type;
			this.Thread = GetThreadInfo();
			this.Source = Source;
			this.Name = Name;
			this.Message = Message;
			this.eException = (eException == null) ? "" : eException.ToString();
		}

		private string GetThreadInfo()
		{
			return GetThreadInfo(System.Threading.Thread.CurrentThread);
		}

		private string GetThreadInfo(System.Threading.Thread t)
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
