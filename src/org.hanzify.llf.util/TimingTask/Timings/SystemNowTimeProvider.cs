
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class SystemNowTimeProvider : INowTimeProvider
	{
		private static INowTimeProvider _Instance = new SystemNowTimeProvider();

		public static INowTimeProvider Instance
		{
			get { return _Instance; }
		}

		private SystemNowTimeProvider() {}

		public DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}
