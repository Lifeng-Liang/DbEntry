using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public abstract class FullType : DbObjectModel<FullType>
    {
        public abstract string c1 { get; set; }
        public abstract int c2 { get; set; }
        public abstract short c3 { get; set; }
        public abstract byte c4 { get; set; }
        public abstract bool c5 { get; set; }
        public abstract DateTime c6 { get; set; }
        public abstract decimal c7 { get; set; }
        public abstract float c8 { get; set; }
        public abstract double c9 { get; set; }
        public abstract Guid c10 { get; set; }
        public abstract sbyte c11 { get; set; }
        public abstract byte[] c15 { get; set; }
    }

    [TestFixture]
    public class FullTypesTest
    {
        private static readonly DbContext sqlite = new DbContext("SQLite");
        private static readonly Guid guid = Guid.NewGuid();

        static FullTypesTest()
        {
            ClassHelper.CallFunction(sqlite, "TryCreateTable", typeof(FullType));
        }

        [SetUp]
        public void SetUp()
        {
            FullType ft = FullType.New();
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
            ft.c15 = new byte[] { 1, 2, 3, 4, 5 };
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
            List<FullType> ls = sqlite.From<FullType>().Where(WhereCondition.EmptyCondition).Select();
            Assert.AreEqual(1, ls.Count);
            FullType o = ls[0];
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Id);
            Assert.AreEqual("tom", o.c1);
            Assert.AreEqual(2, o.c2);
            Assert.AreEqual(3, o.c3);
            Assert.AreEqual(4, o.c4);
            Assert.AreEqual(true, o.c5);
            Assert.AreEqual(new DateTime(2000, 1, 1), o.c6);
            Assert.AreEqual(7, o.c7);
            Assert.AreEqual(8.1, o.c8);
            Assert.AreEqual(9.1, o.c9);
            Assert.AreEqual(guid, o.c10);
            Assert.AreEqual(11, o.c11);
            Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, o.c15);
        }
    }
}
