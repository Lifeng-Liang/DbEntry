using System;
using Leafing.Core.TimingTask;
using Leafing.Core.TimingTask.Timings;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util.timingTask {
    [TestFixture]
    public class DayTimingTest {
        [Test]
        public void TestIt() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3));

            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(1, 2, 3));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 3, 6, 1, 0, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(2, 2, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(2, 6, 9));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(1, 51, 49));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 3, 6, 7, 12, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());

            t.Reset();
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());
        }

        [Test]
        public void TestIt2() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3));

            MockMiscProvider.MockNow = (new DateTime(2004, 3, 6, 7, 12, 2));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());

            t.Reset();
            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());
        }

        [Test]
        public void TestOverTimingPoint() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 7, 12, 2, 0));
            ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3));

            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 10));
            Assert.AreEqual(true, t.TimesUp());
        }

        [Test]
        public void TestAtTimingPoint() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 7, 12, 5, 0));
            ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3));

            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 10));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 50));
            Assert.AreEqual(false, t.TimesUp());
        }

        [Test]
        public void TestOneDayOnce() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 7, 10, 2, 0));
            ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3));

            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 7, 12, 2, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 10));
            Assert.AreEqual(true, t.TimesUp());

            t.Reset();
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 10));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 20));
            Assert.AreEqual(false, t.TimesUp());
        }
    }
}