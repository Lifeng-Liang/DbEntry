using Lephone.Data;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class LogicalOperatorTest
    {
        [Test]
        public void TestAnd()
        {
            Condition c = CK.K["Id"] > 5 && CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            Assert.AreEqual("([Id] > @Id_0) AND ([Id] < @Id_1)", c.ToSqlText(dpc, new SqlServer2000()));
        }

        [Test]
        public void TestOr()
        {
            Condition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            Assert.AreEqual("([Id] > @Id_0) OR ([Id] < @Id_1)", c.ToSqlText(dpc, new SqlServer2000()));
        }

        [Test]
        public void TestTwoWay()
        {
            Condition c = CK.K["Id"] > 5 || CK.K["Id"] < 9;
            Condition c1 = CK.K["Id"] > 5 | CK.K["Id"] < 9;
            var dpc = new DataParameterCollection();
            var dpc1 = new DataParameterCollection();
            Assert.AreEqual(c.ToSqlText(dpc, new SqlServer2000()), c1.ToSqlText(dpc1, new SqlServer2000()));
        }
    }
}
