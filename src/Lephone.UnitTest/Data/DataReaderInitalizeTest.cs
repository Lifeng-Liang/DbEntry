using System.Linq;
using System.Data;
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

            public IDataReaderInitalize Initalize(IDataReader dr, int startIndex)
            {
                Name = (string)dr["Name"];
                Count = (int) dr["Count"];
                return this;
            }

            public int FieldCount
            {
                get { return 2; }
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

            var result = DbEntry.From<DrInit>().Where(Condition.Empty).GroupBy<Cols>("Name,Count");
            var list = (from p in result orderby p.Column.Count select p).ToList();

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual("tom", list[0].Column.Name);
            Assert.AreEqual(1, list[0].Column.Count);

            Assert.AreEqual("tom", list[1].Column.Name);
            Assert.AreEqual(2, list[1].Column.Count);

            Assert.AreEqual("jerry", list[2].Column.Name);
            Assert.AreEqual(3, list[2].Column.Count);
        }
    }
}
