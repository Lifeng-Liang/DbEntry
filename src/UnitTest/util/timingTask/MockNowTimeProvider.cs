
#region usings

using System;
using org.hanzify.llf.util.TimingTask.Timings;

#endregion

namespace org.hanzify.llf.UnitTest.util.timingTask
{
	public class MockNowTimeProvider : INowTimeProvider
	{
		private DateTime _Now = DateTime.MinValue;

		public MockNowTimeProvider() {}

		public MockNowTimeProvider(DateTime dt)
		{
			SetNow(dt);
		}

		public DateTime Now
		{
			get { return _Now; }
		}

		public void SetNow(DateTime dt)
		{
			_Now = dt;
		}

		public void Add(TimeSpan ts)
		{
			_Now = _Now.Add(ts);
		}
	}
}
