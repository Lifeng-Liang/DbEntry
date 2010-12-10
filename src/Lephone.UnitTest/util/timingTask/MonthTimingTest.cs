using System;
using Lephone.Core.TimingTask;
using Lephone.Core.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class MonthTimingTest
	{
		[Test]
		public void TestIt()
		{
            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 7, 7, 10, 2, 0));
			ITiming t = new MonthTiming(new TimeOfDayStructure(8, 0, 0), 8);

			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 7, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 8, 7, 59, 59, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 8, 8, 0, 0, 0));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 8, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 9, 7, 59, 59, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Me.SetNow(new DateTime(2004, 1, 9, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());
		}
	}
}
