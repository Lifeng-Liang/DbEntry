using System;
using System.Text.RegularExpressions;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.UnitTest.Data.Objects;
using Lephone.Core;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
    #region objects

    class NotPublic : DbObject
    {
        public string Name;
    }

    [Serializable]
    public class TableA : DbObjectModel<TableA>
    {
        public string Name { get; set; }

        [HasOne]
        public TableB tableB { get; set; }
    }

    [Serializable]
    public class TableB : DbObjectModel<TableB>
    {
        [Index(UNIQUE = true, IndexName = "Url_TableAId", ASC = false)]
        public string Url { get; set; }

        [Index(UNIQUE = true, IndexName = "Url_TableAId", ASC = false)]
        [BelongsTo, DbColumn("TableAId")]
        public TableA TB { get; set; }
    }

    public class ClassSite {}

    public class indexSample
    {
        [Index(UNIQUE = true, IndexName = "indexname1", ASC = true)]
        [Index(UNIQUE = false, IndexName = "indexname2", ASC = true)]
        public string Name { get; set; }

        [Index(UNIQUE = true, IndexName = "indexname1", ASC = true)]
        [Index(UNIQUE = false, IndexName = "indexname2", ASC = true)]
        [BelongsTo, DbColumn("SiteId")]
        public ClassSite Site { get; set; }

        [Index(UNIQUE = true, IndexName = "qid", ASC = true)]
        [BelongsTo, DbColumn("SiteId"), Index(IndexName = "xxx"), Index(IndexName = "ccc")]
        public ClassSite Site2 { get; set; }
    }

    #endregion

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
            var Exp = new OrderBy((DESC)"Id", (ASC)"Name");
            var Dst = OrderBy.Parse(s);
            var ds = new DataParameterCollection();
            string ExpStr = Exp.ToSqlText(ds, DbEntry.Context.Dialect);
            string DstStr = Dst.ToSqlText(ds, DbEntry.Context.Dialect);
            Assert.AreEqual(ExpStr, DstStr);
        }

        [Test]
        public void TestCloneObject()
        {
            var p = new People {Id = 10, Name = "abc", pc = new PCs {Name = "uuu"}};

            var p1 = (People)ObjectInfo.CloneObject(p);
            Assert.AreEqual(10, p1.Id);
            Assert.AreEqual("abc", p1.Name);
            // Assert.IsNull(p1.pc);
        }

        [Test]
        public void TestBaseType()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(People));
            Assert.AreEqual("People", oi.HandleType.Name);
        }

        [Test]
        public void TestBaseType2()
        {
            Type t = new People().GetType();
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            Assert.AreEqual("People", oi.HandleType.Name);
        }

        [Test]
        public void TestIndexes()
        {
            var t = typeof (indexSample);
            var f = t.GetProperty("Name");
            var os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(2, os.Length);
        }

        [Test]
        public void TestIndexes2()
        {
            var t = typeof(indexSample);
            var f = t.GetProperty("Site");
            var os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(2, os.Length);

            f = t.GetProperty("Site2");
            os = ClassHelper.GetAttributes<IndexAttribute>(f, false);
            Assert.AreEqual(3, os.Length);
        }

        [Test]
        public void TestX()
        {
            DbContext de = DbEntry.Context;
            de.DropAndCreate(typeof(TableA));
            de.DropAndCreate(typeof(TableB));

            var t1 = new TableA {Name = "TestName1"};
            t1.Save();

            var t2 = TableA.FindById(1);
            var t3 = new TableB {Url = "TestUrl1", TB = t2};
            t3.Validate();
            t3.Save();
        }

        [Test]
        public void TestNotPublicClass()
        {
            try
            {
                var obj = new NotPublic {Name = "tom"};
                DbEntry.Context.Insert(obj);
                Assert.IsTrue(false);
            }
            catch (DataException ex)
            {
                Assert.AreEqual("[Lephone.UnitTest.Data.Inner.NotPublic]The model class should be public.", ex.Message);
            }
        }
    }
}
