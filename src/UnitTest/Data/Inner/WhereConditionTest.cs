
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.UnitTest.Data.Inner
{
    [TestFixture]
    public class WhereConditionTest
    {
        [Test]
        public void Test1()
        {
            WhereCondition c = null;
            c = ((c & null) | null);
            Assert.IsTrue(c is EmptyCondition);
        }

        [Test]
        public void Test2()
        {
            WhereCondition c = null;
            c = ((c & null) | null);
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual("", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test3()
        {
            WhereCondition c = WhereCondition.EmptyCondition;
            c = ((c & null) | null);
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual("", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test4()
        {
            WhereCondition c = WhereCondition.EmptyCondition;
            c &= (CK.K["Id"] == 1 | CK.K["Age"] > 18);
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.IsFalse(c is EmptyCondition);
            Assert.AreEqual(" Where ([Id] = @Id_0) Or ([Age] > @Age_1)", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test5()
        {
            WhereCondition c = WhereCondition.EmptyCondition;
            c = c.And(CK.K["Id"].Eq(1)).Or(CK.K["Age"].Gt(18));
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual(" Where ([Id] = @Id_0) Or ([Age] > @Age_1)", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test6()
        {
            WhereCondition c = null;
            c &= (CK.K["Id"] == 1 | CK.K["Age"] > 18);
            c &= null;
            c |= null;
            c &= CK.K["Gender"] == true;
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual(" Where (([Id] = @Id_0) Or ([Age] > @Age_1)) And ([Gender] = @Gender_2)", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test7()
        {
            WhereCondition c = WhereCondition.EmptyCondition;
            c = c.And(CK.K["Id"].Eq(1)).Or(CK.K["Age"].Gt(18));
            c = c.And(null);
            c = c.Or(null);
            c = c.And(CK.K["Gender"].Eq(true));
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual(" Where (([Id] = @Id_0) Or ([Age] > @Age_1)) And ([Gender] = @Gender_2)", cc.ToSqlText(ref dpc, new Access()));
        }

        [Test]
        public void Test8()
        {
            WhereCondition c = WhereCondition.EmptyCondition;
            c = c.And(CK.K["Id"] == 1).Or(CK.K["Age"] > 18);
            c = c.And(null);
            c = c.Or(null);
            c = c.And(CK.K["Gender"].Eq(true));
            WhereClause cc = new WhereClause(c);
            DataParamterCollection dpc = new DataParamterCollection();
            Assert.AreEqual(" Where (([Id] = @Id_0) Or ([Age] > @Age_1)) And ([Gender] = @Gender_2)", cc.ToSqlText(ref dpc, new Access()));
        }
    }
}
