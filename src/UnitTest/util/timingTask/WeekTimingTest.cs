
#region usings

using System;
using NUnit.Framework;
using org.hanzify.llf.util.TimingTask;
using org.hanzify.llf.util.TimingTask.Timings;

#endregion

namespace org.hanzify.llf.UnitTest.util.timingTask
{
	[TestFixture]
	public class WeekTimingTest
	{
		[Test]
		public void TestIt()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,11,21,7,10,2,0));
			ITiming t = new WeekTiming(new TimeOfDayStructure(7, 12, 3), DayOfWeek.Monday, ntp);

			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,22,7,10,2));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,22,7,12,2));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,22,7,12,3));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,23,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,24,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,25,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,26,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,27,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,28,7,12,4));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,11,29,7,12,4));
			Assert.AreEqual(true, t.TimesUp());
		}
	}
}
