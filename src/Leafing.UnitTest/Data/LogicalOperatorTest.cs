using Leafing.Data;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class LogicalOperatorTest
    {
        [Test]
        public void TestAnd()
        {
            Condition c = CK.K["Id"] > 5 && CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            Assert.AreEqual("([Id] > @Id_0) AND ([Id] < @Id_1)", c.ToSqlText(dpc, new SqlServer2000(), null));
        }

        [Test]
        public void TestOr()
        {
            Condition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            Assert.AreEqual("([Id] > @Id_0) OR ([Id] < @Id_1)", c.ToSqlText(dpc, new SqlServer2000(), null));
        }

        [Test]
        public void TestTwoWay()
        {
            Condition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            Condition c1 = CK.K["Id"] > 5 | CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            var dpc1 = new DataParameterCollection();
            Assert.AreEqual(c.ToSqlText(dpc, new SqlServer2000(), null), c1.ToSqlText(dpc1, new SqlServer2000(), null));
        }
    }
}
