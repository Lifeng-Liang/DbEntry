using System;
using Lephone.Data.Definition;
using Lephone.Linq;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DbTable("DateAndTime")]
    public abstract class DateAndTime : LinqObjectModel<DateAndTime>
    {
        public abstract DateTime dtValue { get; set; }

        public abstract Date dValue { get; set; }

        public abstract Time tValue { get; set; }

        public abstract DateTime? dtnValue { get; set; }

        public abstract Date? dnValue { get; set; }

        public abstract Time? tnValue { get; set; }
    }

    public abstract class DateAndTime2 : LinqObjectModel<DateAndTime2>
    {
        public abstract Date StartDate { get; set; }
        public abstract Time StartTime { get; set; }

        public abstract DateAndTime2 Init(Date startDate, Time startTime);
    }


    [TestFixture]
    public class DateTimeTest : DataTestBase
    {
        [Test]
        public void TestSelect1()
        {
            var o = DateAndTime.FindById(1);
            Assert.IsNotNull(o);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), o.dtValue);
            Assert.AreEqual(Date.Parse("2004-8-19"), o.dValue);
            Assert.AreEqual(Time.Parse("18:51:06"), o.tValue);
            Assert.IsNull(o.dtnValue);
            Assert.IsNull(o.dnValue);
            Assert.IsNull(o.tnValue);
        }

        [Test]
        public void TestSelect2()
        {
            var o = DateAndTime.FindById(2);
            Assert.IsNotNull(o);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), o.dtValue);
            Assert.AreEqual(Date.Parse("2004-8-19"), o.dValue);
            Assert.AreEqual(Time.Parse("18:51:06"), o.tValue);
            Assert.AreEqual(DateTime.Parse("2004-8-19 18:51:06"), o.dtnValue.Value);
            Assert.AreEqual(Date.Parse("2004-8-19"), o.dnValue.Value);
            Assert.AreEqual(Time.Parse("18:51:06"), o.tnValue.Value);
        }

        [Test]
        public void Test3()
        {
            var dt = DateAndTime2.New.Init(new Date(2008, 7, 22), new Time(12, 30, 50));
            dt.Save();

            var n = DateAndTime2.FindById(dt.Id);
            Assert.IsNotNull(n);
            Assert.AreEqual(new Date(2008, 7, 22), n.StartDate);
            Assert.AreEqual(new Time(12, 30, 50), n.StartTime);
        }
    }
}
