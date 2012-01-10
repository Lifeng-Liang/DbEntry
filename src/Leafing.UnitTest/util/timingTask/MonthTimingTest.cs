using System;
using Leafing.Core.TimingTask;
using Leafing.Core.TimingTask.Timings;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util.timingTask
{
	[TestFixture]
	public class MonthTimingTest
	{
		[Test]
		public void TestIt()
		{
            MockMiscProvider.MockNow = (new DateTime(2004, 1, 7, 7, 10, 2, 0));
			ITiming t = new MonthTiming(new TimeOfDayStructure(8, 0, 0), 8);

			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 7, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 8, 7, 59, 59, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 8, 8, 0, 0, 0));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 8, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 9, 7, 59, 59, 0));
			Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 1, 9, 8, 0, 0, 0));
			Assert.AreEqual(false, t.TimesUp());
		}
	}
}
