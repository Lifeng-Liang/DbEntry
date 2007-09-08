
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Lephone.Data;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;

#endregion

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

            string s = "Id desc, Name";
            OrderBy Exp = new OrderBy((DESC)"Id", (ASC)"Name");
            OrderBy Dst = OrderBy.Parse(s);
            DataParamterCollection ds = new DataParamterCollection();
            string ExpStr = Exp.ToSqlText(ref ds, DbEntry.Context.Dialect);
            string DstStr = Dst.ToSqlText(ref ds, DbEntry.Context.Dialect);
            Assert.AreEqual(ExpStr, DstStr);
        }
    }
}
