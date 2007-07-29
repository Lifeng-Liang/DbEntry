
#region usings

using System;
using NUnit.Framework;
using org.hanzify.llf.util.TimingTask;
using org.hanzify.llf.util.TimingTask.Timings;

#endregion

namespace org.hanzify.llf.UnitTest.util.timingTask
{
	[TestFixture]
	public class MonthTimingTest
	{
		[Test]
		public void TestIt()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,1,7,7,10,2,0));
			ITiming t = new MonthTiming(new TimeOfDayStructure(8, 0, 0), 8, ntp);

			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,7,8,0,0,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,8,7,59,59,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,8,8,0,0,0));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,8,8,0,0,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,9,7,59,59,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,1,9,8,0,0,0));
			Assert.AreEqual(false, t.TimesUp());
		}
	}
}
