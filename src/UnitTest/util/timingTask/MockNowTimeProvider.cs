
#region usings

using System;
using Lephone.Util.TimingTask.Timings;

#endregion

namespace Lephone.UnitTest.util.timingTask
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
