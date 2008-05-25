using System;
using NUnit.Framework;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data
{
    [DbTable("DateAndTime")]
    public abstract class DateAndTime : DbObjectModel<DateAndTime>
    {
        public abstract DateTime dtValue { get; set; }

        public abstract Date dValue { get; set; }

        public abstract Time tValue { get; set; }

        public abstract DateTime? dtnValue { get; set; }

        public abstract Date? dnValue { get; set; }

        public abstract Time? tnValue { get; set; }
    }

    [TestFixture]
    public class DateTimeTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

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
    }
}
