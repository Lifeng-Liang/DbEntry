using Lephone.Data;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
    [TestFixture]
    public class WhereConditionTest
    {
        [Test]
        public void Test1()
        {
            Condition c = null;
            c = ((c & null) | null);
            Assert.IsTrue(c is EmptyCondition);
        }

        [Test]
        public void Test2()
        {
            Condition c = null;
            c = ((c & null) | null);
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual("", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test3()
        {
            Condition c = Condition.Empty;
            c = ((c & null) | null);
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual("", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test4()
        {
            Condition c = Condition.Empty;
            c &= (CK.K["Id"] == 1 | CK.K["Age"] > 18);
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.IsFalse(c is EmptyCondition);
            Assert.AreEqual(" WHERE ([Id] = @Id_0) OR ([Age] > @Age_1)", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test5()
        {
            Condition c = Condition.Empty;
            c = c.And(CK.K["Id"].Eq(1)).Or(CK.K["Age"].Gt(18));
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual(" WHERE ([Id] = @Id_0) OR ([Age] > @Age_1)", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test6()
        {
            Condition c = null;
            c &= (CK.K["Id"] == 1 | CK.K["Age"] > 18);
            c &= null;
            c |= null;
            c &= CK.K["Gender"] == true;
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual(" WHERE (([Id] = @Id_0) OR ([Age] > @Age_1)) AND ([Gender] = @Gender_2)", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test7()
        {
            Condition c = Condition.Empty;
            c = c.And(CK.K["Id"].Eq(1)).Or(CK.K["Age"].Gt(18));
            c = c.And(null);
            c = c.Or(null);
            c = c.And(CK.K["Gender"].Eq(true));
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual(" WHERE (([Id] = @Id_0) OR ([Age] > @Age_1)) AND ([Gender] = @Gender_2)", cc.ToSqlText(dpc, new Access()));
        }

        [Test]
        public void Test8()
        {
            Condition c = Condition.Empty;
            c = c.And(CK.K["Id"] == 1).Or(CK.K["Age"] > 18);
            c = c.And(null);
            c = c.Or(null);
            c = c.And(CK.K["Gender"].Eq(true));
            var cc = new WhereClause(c);
            var dpc = new DataParameterCollection();
            Assert.AreEqual(" WHERE (([Id] = @Id_0) OR ([Age] > @Age_1)) AND ([Gender] = @Gender_2)", cc.ToSqlText(dpc, new Access()));
        }
    }
}
