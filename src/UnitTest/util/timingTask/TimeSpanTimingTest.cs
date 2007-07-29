
#region usings

using System;
using NUnit.Framework;
using org.hanzify.llf.util.TimingTask;
using org.hanzify.llf.util.TimingTask.Timings;

#endregion

namespace org.hanzify.llf.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimeSpanTimingTest
	{
		[Test]
		public void TestForSecends()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(0, 0, 3), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,4));
			Assert.AreEqual(true, t.TimesUp());
		}

		[Test]
		public void TestForMinutes()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(0, 5, 0), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,1,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,3,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,58));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,5,10));
			Assert.AreEqual(true, t.TimesUp());
		}

		[Test]
		public void TestForHours()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(6, 0, 0), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,1,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(1,3,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(4,55,58));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(6,0,8));
			Assert.AreEqual(true, t.TimesUp());
		}
	}
}
