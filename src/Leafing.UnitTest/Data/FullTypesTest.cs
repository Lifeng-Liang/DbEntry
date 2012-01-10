using System;
using System.Reflection;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [DbContext("SQLite")]
    public class FullType : DbObjectModel<FullType>
    {
        public string c1 { get; set; }
        public int c2 { get; set; }
        public short c3 { get; set; }
        public byte c4 { get; set; }
        public bool c5 { get; set; }
        public DateTime c6 { get; set; }
        public decimal c7 { get; set; }
        public float c8 { get; set; }
        public double c9 { get; set; }
        public Guid c10 { get; set; }
        public sbyte c11 { get; set; }
        public byte[] c15 { get; set; }
    }

    [DbContext("SQLite")]
    public class FullType2 : DbObjectModel<FullType2>
    {
        public string c1 { get; set; }
        public int? c2 { get; set; }
        public short? c3 { get; set; }
        public byte? c4 { get; set; }
        public bool? c5 { get; set; }
        public DateTime? c6 { get; set; }
        public decimal? c7 { get; set; }
        public float? c8 { get; set; }
        public double? c9 { get; set; }
        public Guid? c10 { get; set; }
        public sbyte? c11 { get; set; }
        public byte[] c15 { get; set; }
    }

    [TestFixture]
    public class FullTypesTest
    {
        private static readonly Guid guid = Guid.NewGuid();

        static FullTypesTest()
        {
            var ft = ModelContext.GetInstance(typeof(FullType));
            ft.Operator.TryCreateTable();
            var ft2 = ModelContext.GetInstance(typeof(FullType2));
            ft2.Operator.TryCreateTable();
        }

        [SetUp]
        public void SetUp()
        {
            var ft = new FullType
                         {
                             c1 = "tom",
                             c2 = 2,
                             c3 = 3,
                             c4 = 4,
                             c5 = true,
                             c6 = new DateTime(2000, 1, 1),
                             c7 = 7,
                             c8 = (float) 8.1,
                             c9 = 9.1,
                             c10 = guid,
                             c11 = 11,
                             c15 = new byte[] {1, 2, 3, 4, 5}
                         };
            // get infos.
            PropertyInfo[] pis = typeof(FullType).GetProperties();
            StaticRecorder.CurRow.Clear();
            StaticRecorder.CurRow.Add(new RowInfo("Id", typeof(long), 1L));
            foreach (PropertyInfo pi in pis)
            {
                object o = pi.GetValue(ft, null);
                StaticRecorder.CurRow.Add(new RowInfo(pi.Name, pi.PropertyType, o));
            }
        }

        [Test]
        public void Test1()
        {
            var ls = DbEntry.From<FullType>().Where(Condition.Empty).Select();
            Assert.AreEqual(1, ls.Count);
            var o = ls[0];
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Id);
            Assert.AreEqual("tom", o.c1);
            Assert.AreEqual(2, o.c2);
            Assert.AreEqual(3, o.c3);
            Assert.AreEqual(4, o.c4);
            Assert.AreEqual(true, o.c5);
            Assert.AreEqual(new DateTime(2000, 1, 1), o.c6);
            Assert.AreEqual(7, o.c7);
            Assert.AreEqual(8.1f, o.c8);
            Assert.AreEqual(9.1d, o.c9);
            Assert.AreEqual(guid, o.c10);
            Assert.AreEqual(11, o.c11);
            Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, o.c15);
        }

        [Test]
        public void Test2()
        {
            var ls = DbEntry.From<FullType2>().Where(Condition.Empty).Select();
            Assert.AreEqual(1, ls.Count);
            var o = ls[0];
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Id);
            Assert.AreEqual("tom", o.c1);
            Assert.AreEqual(2, o.c2);
            Assert.AreEqual(3, o.c3);
            Assert.AreEqual(4, o.c4);
            Assert.AreEqual(true, o.c5);
            Assert.AreEqual(new DateTime(2000, 1, 1), o.c6);
            Assert.AreEqual(7, o.c7);
            Assert.AreEqual(8.1f, o.c8);
            Assert.AreEqual(9.1d, o.c9);
            Assert.AreEqual(guid, o.c10);
            Assert.AreEqual(11, o.c11);
            Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, o.c15);
        }

        [Test]
        public void TestSameValueIsNotChange()
        {
            var ls = DbEntry.From<FullType>().Where(Condition.Empty).Select();
            StaticRecorder.ClearMessages();
            Assert.AreEqual(1, ls.Count);
            var ft = ls[0];
            ft.c1 = "tom";
            ft.c2 = 2;
            ft.c3 = 3;
            ft.c4 = 4;
            ft.c5 = true;
            ft.c6 = new DateTime(2000, 1, 1);
            ft.c7 = 7;
            ft.c8 = (float)8.1;
            ft.c9 = 9.1;
            ft.c10 = guid;
            ft.c11 = 11;
            ft.Save();
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
        }

        [Test]
        public void TestSameValueIsNotChange2()
        {
            var ls = DbEntry.From<FullType2>().Where(Condition.Empty).Select();
            StaticRecorder.ClearMessages();
            Assert.AreEqual(1, ls.Count);
            var ft = ls[0];
            ft.c1 = "tom";
            ft.c2 = 2;
            ft.c3 = 3;
            ft.c4 = 4;
            ft.c5 = true;
            ft.c6 = new DateTime(2000, 1, 1);
            ft.c7 = 7;
            ft.c8 = (float)8.1;
            ft.c9 = 9.1;
            ft.c10 = guid;
            ft.c11 = 11;
            ft.Save();
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
        }
    }
}
