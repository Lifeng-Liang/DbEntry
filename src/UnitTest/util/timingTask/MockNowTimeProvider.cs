using System;
using Lephone.Util;

namespace Lephone.UnitTest.util.timingTask
{
	public class MockNowTimeProvider : NowProvider
	{
		private DateTime _Now = DateTime.MinValue;

		public MockNowTimeProvider() {}

		public MockNowTimeProvider(DateTime dt)
		{
			SetNow(dt);
		}

		public override DateTime Now
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
