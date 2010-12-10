using System;
using Lephone.Core.TimingTask;
using Lephone.Core.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class WeekTimingTest
	{
		[Test]
		public void TestIt()
		{
            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 21, 7, 10, 2, 0));
			ITiming t = new WeekTiming(new TimeOfDayStructure(7, 12, 3), DayOfWeek.Monday);

			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 22, 7, 10, 2));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 22, 7, 12, 2));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 22, 7, 12, 3));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 23, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 24, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 25, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 26, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 27, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 28, 7, 12, 4));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 11, 29, 7, 12, 4));
			Assert.AreEqual(true, t.TimesUp());
		}
	}
}
