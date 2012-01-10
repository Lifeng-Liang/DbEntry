using System;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [DbTable("DateAndTime")]
    public class DateAndTime : DbObjectModel<DateAndTime>
    {
        public DateTime dtValue { get; set; }

        public Date dValue { get; set; }

        public Time tValue { get; set; }

        public DateTime? dtnValue { get; set; }

        public Date? dnValue { get; set; }

        public Time? tnValue { get; set; }
    }

    [DbTable("DateAndTime"), DbContext("SQLite")]
    public class DateAndTimeSqlite : DbObjectModel<DateAndTimeSqlite>
    {
        public DateTime dtValue { get; set; }

        public Date dValue { get; set; }

        public Time tValue { get; set; }

        public DateTime? dtnValue { get; set; }

        public Date? dnValue { get; set; }

        public Time? tnValue { get; set; }
    }

    public class DateAndTime2 : DbObjectModel<DateAndTime2>
    {
        public Date StartDate { get; set; }
        public Time StartTime { get; set; }
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
            var dt = new DateAndTime2 {StartDate = new Date(2008, 7, 22), StartTime = new Time(12, 30, 50)};
            dt.Save();

            var n = DateAndTime2.FindById(dt.Id);
            Assert.IsNotNull(n);
            Assert.AreEqual(new Date(2008, 7, 22), n.StartDate);
            Assert.AreEqual(new Time(12, 30, 50), n.StartTime);
        }
    }
}
