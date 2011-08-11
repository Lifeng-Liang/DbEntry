using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [DbTable("ArticleMore")]
    public class ArticleMore : DbObjectModel<ArticleMore>
    {
        public string Name { get; set; }

        [HasMany(OrderBy = "Id")]
        public IList<BelongsMore> Bms { get; private set; }
    }

    [DbTable("ReaderMore")]
    public class ReaderMore : DbObjectModel<ReaderMore>
    {
        public string Name { get; set; }

        [HasMany(OrderBy = "Id")]
        public IList<BelongsMore> Bms { get; private set; }
    }

    [DbTable("BelongsMore")]
    public class BelongsMore : DbObjectModel<BelongsMore>
    {
        [Length(50)]
        public string Name { get; set; }

        [BelongsTo, DbColumn("Article_Id")]
        public ArticleMore Article { get; set; }

        [BelongsTo, DbColumn("Reader_Id")]
        public ReaderMore Reader { get; set; }
    }

    #endregion

    [TestFixture]
    public class BelongsMoreTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            ArticleMore a = ArticleMore.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("f1", a.Bms[0].Name);


            a = ArticleMore.FindById(2);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("f2", a.Bms[0].Name);

            a = ArticleMore.FindById(3);
            Assert.AreEqual(2, a.Bms.Count);
            Assert.AreEqual("f3", a.Bms[0].Name);
            Assert.AreEqual("f4", a.Bms[1].Name);
        }

        [Test]
        public void Test2()
        {
            ReaderMore r = ReaderMore.FindById(1);
            Assert.IsNotNull(r);
            Assert.AreEqual("tom", r.Name);
            Assert.AreEqual(1, r.Bms.Count);
            Assert.AreEqual("f3", r.Bms[0].Name);

            r = ReaderMore.FindById(2);
            Assert.AreEqual(1, r.Bms.Count);
            Assert.AreEqual("f1", r.Bms[0].Name);

            r = ReaderMore.FindById(3);
            Assert.AreEqual(2, r.Bms.Count);
            Assert.AreEqual("f2", r.Bms[0].Name);
            Assert.AreEqual("f4", r.Bms[1].Name);
        }

        [Test]
        public void Test3()
        {
            BelongsMore b = BelongsMore.FindById(1);
            Assert.IsNotNull(b);
            Assert.AreEqual("The lovely bones", b.Article.Name);
            Assert.AreEqual("jerry", b.Reader.Name);

            b = BelongsMore.FindById(3);
            Assert.AreEqual("The load of rings", b.Article.Name);
            Assert.AreEqual("tom", b.Reader.Name);
        }

        [Test]
        public void TestInsert()
        {
            var r = new ReaderMore {Name = "test"};
            r.Save();

            ReaderMore r1 = ReaderMore.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(0, r1.Bms.Count);
        }

        [Test]
        public void TestInsertRelations()
        {
            var r = new ReaderMore {Name = "test"};
            var b = new BelongsMore {Name = "b"};
            r.Bms.Add(b);
            r.Save();

            ReaderMore r1 = ReaderMore.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(1, r1.Bms.Count);
            Assert.AreEqual("b", r1.Bms[0].Name);

            Assert.AreEqual("test", r1.Bms[0].Reader.Name);
            Assert.IsNull(r1.Bms[0].Article);
        }

        [Test]
        public void TestInsertRelations2()
        {
            var r = new ReaderMore {Name = "test"};
            var a = new ArticleMore {Name = "art"};
            var b = new BelongsMore {Name = "b"};
            r.Bms.Add(b);
            a.Bms.Add(b);
            r.Save();
            //a.Save();

            ReaderMore r1 = ReaderMore.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(1, r1.Bms.Count);
            Assert.AreEqual("b", r1.Bms[0].Name);
            Assert.AreEqual("test", r1.Bms[0].Reader.Name);
            Assert.AreEqual("art", r1.Bms[0].Article.Name);
        }

        [Test]
        public void TestUpdate()
        {
            ArticleMore a = ArticleMore.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("f1", a.Bms[0].Name);

            a.Name = "haha";
            a.Bms[0].Name = "ok";
            a.Save();

            a = ArticleMore.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("haha", a.Name);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("ok", a.Bms[0].Name);
        }

        [Test]
        public void TestUpdate2()
        {
            ArticleMore a = ArticleMore.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("f1", a.Bms[0].Name);

            a.Bms[0].Name = "ok";
            a.Save();

            a = ArticleMore.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("ok", a.Bms[0].Name);
        }

        [Test]
        public void TestUpdate3()
        {
            ArticleMore a1 = ArticleMore.FindById(1);
            ArticleMore a2 = ArticleMore.FindById(2);
            a2.Bms.Add(a1.Bms[0]);
            a2.Save();

            a1 = ArticleMore.FindById(1);
            Assert.IsNotNull(a1);
            Assert.AreEqual("The lovely bones", a1.Name);
            Assert.AreEqual(0, a1.Bms.Count);

            a2 = ArticleMore.FindById(2);
            Assert.AreEqual(2, a2.Bms.Count);
            Assert.AreEqual("f1", a2.Bms[0].Name);
            Assert.AreEqual("f2", a2.Bms[1].Name);
        }

        [Test]
        public void TestDelete()
        {
            ArticleMore.FindById(1).Delete();

            ReaderMore r = ReaderMore.FindById(2);
            Assert.AreEqual("f1", r.Bms[0].Name);
            Assert.IsNull(r.Bms[0].Article);
        }

        [Test]
        public void TestDelete2()
        {
            ArticleMore.FindById(1).Delete();
            // it's don't need to be.
            // Assert.AreEqual(0, DbEntry.Provider.ExecuteScalar("select [Article_Id] from [BelongsMore] Where [Id] = 1"));
        }

        [Test]
        public void TestDelete3()
        {
            ArticleMore a = ArticleMore.FindById(1);
            Assert.AreEqual("f1", a.Bms[0].Name);
            a.Delete();

            Assert.IsNull(ArticleMore.FindById(1));
            ReaderMore r = ReaderMore.FindById(2);
            Assert.AreEqual(1, r.Bms.Count);
            Assert.AreEqual("f1", r.Bms[0].Name);
        }

        [Test]
        public void TestCreateTable()
        {
            DbEntry.DropAndCreate(typeof(BelongsMore));
            ArticleMore a = ArticleMore.FindById(1);
            Assert.AreEqual(0, a.Bms.Count);

            a.Bms.Add(new BelongsMore {Name = "mytest"});
            a.Save();

            a = ArticleMore.FindById(1);
            Assert.AreEqual(1, a.Bms.Count);
            Assert.AreEqual("mytest", a.Bms[0].Name);
        }
    }
}
