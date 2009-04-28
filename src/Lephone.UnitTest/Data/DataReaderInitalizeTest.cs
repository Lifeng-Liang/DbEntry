using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class DataReaderInitalizeTest : DataTestBase
    {
        public class DrInit : DbObject
        {
            public string Name;
            public int Count;
        }

        public class Cols : IDataReaderInitalize
        {
            public string Name;
            public int Count;

            public void Initalize(IDataReader dr)
            {
                Name = (string)dr["Name"];
                Count = (int) dr["Count"];
            }
        }

        [Test]
        public void Test1()
        {
            var x = new DrInit {Name = "tom", Count = 1};
            DbEntry.Save(x);
            x = new DrInit { Name = "tom", Count = 1 };
            DbEntry.Save(x);
            x = new DrInit { Name = "tom", Count = 2 };
            DbEntry.Save(x);
            x = new DrInit { Name = "jerry", Count = 3 };
            DbEntry.Save(x);

            var list = DbEntry.From<DrInit>().Where(null).GroupBy<Cols>("Name,Count");
            Assert.AreEqual(3, list.Count);

            Assert.AreEqual("tom", list[0].Column.Name);
            Assert.AreEqual(1, list[0].Column.Count);

            Assert.AreEqual("tom", list[0].Column.Name);
            Assert.AreEqual(2, list[0].Column.Count);

            Assert.AreEqual("jerry", list[0].Column.Name);
            Assert.AreEqual(3, list[0].Column.Count);
        }
    }
}
