using System;
using System.Text.RegularExpressions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
    [TestFixture]
    public class CommonTest
    {
        [Test]
        public void Test1()
        {
            Assert.IsTrue(Regex.IsMatch("https://localhost", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("http://a.b.c", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("https://a.b.c/", CommonRegular.UrlRegular));
            Assert.IsTrue(Regex.IsMatch("http://a.b.c/a.html?a=bcd&e=12.3", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("httpss://a.b.c", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("a.http://a.b.c", CommonRegular.UrlRegular));
            Assert.IsFalse(Regex.IsMatch("http://a.b.c/a.html?a=bcd&e=aaa()", CommonRegular.UrlRegular));
        }

        [Test]
        public void TestOrderByParse()
        {
            Assert.IsNull(OrderBy.Parse(""));
            Assert.IsNull(OrderBy.Parse(null));

            const string s = "Id desc, Name";
            OrderBy Exp = new OrderBy((DESC)"Id", (ASC)"Name");
            OrderBy Dst = OrderBy.Parse(s);
            DataParamterCollection ds = new DataParamterCollection();
            string ExpStr = Exp.ToSqlText(ds, DbEntry.Context.Dialect);
            string DstStr = Dst.ToSqlText(ds, DbEntry.Context.Dialect);
            Assert.AreEqual(ExpStr, DstStr);
        }

        [Test]
        public void TestCloneObject()
        {
            People p = People.New();
            p.Id = 10;
            p.Name = "abc";
            PCs pc = PCs.New();
            pc.Name = "uuu";
            p.pc = pc;

            People p1 = (People)ObjectInfo.CloneObject(p);
            Assert.AreEqual(10, p1.Id);
            Assert.AreEqual("abc", p1.Name);
            // Assert.IsNull(p1.pc);
        }

        [Test]
        public void TestBaseType()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(People));
            Assert.AreEqual("People", oi.BaseType.Name);
        }

        [Test]
        public void TestBaseType2()
        {
            Type t = People.New().GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            Assert.AreEqual("People", oi.BaseType.Name);
        }
    }
}
