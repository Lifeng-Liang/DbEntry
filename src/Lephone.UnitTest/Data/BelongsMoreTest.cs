using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [DbTable("Article")]
    public class bArticle : DbObjectModel<bArticle>
    {
        public string Name { get; set; }
        [HasMany(OrderBy = "Id")]
        public IList<BelongsMore> bms { get; set; }
    }

    [DbTable("Reader")]
    public class bReader : DbObjectModel<bReader>
    {
        public string Name { get; set; }
        [HasMany(OrderBy = "Id")]
        public IList<BelongsMore> bms { get; set; }
    }

    [DbTable("BelongsMore")]
    public class BelongsMore : DbObjectModel<BelongsMore>
    {
        [Length(50)]
        public string Name { get; set; }
        [BelongsTo]
        public bArticle art { get; set; }
        [BelongsTo]
        public bReader rd { get; set; }
    }

    #endregion

    [TestFixture]
    public class BelongsMoreTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            bArticle a = bArticle.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("f1", a.bms[0].Name);


            a = bArticle.FindById(2);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("f2", a.bms[0].Name);

            a = bArticle.FindById(3);
            Assert.AreEqual(2, a.bms.Count);
            Assert.AreEqual("f3", a.bms[0].Name);
            Assert.AreEqual("f4", a.bms[1].Name);
        }

        [Test]
        public void Test2()
        {
            bReader r = bReader.FindById(1);
            Assert.IsNotNull(r);
            Assert.AreEqual("tom", r.Name);
            Assert.AreEqual(1, r.bms.Count);
            Assert.AreEqual("f3", r.bms[0].Name);

            r = bReader.FindById(2);
            Assert.AreEqual(1, r.bms.Count);
            Assert.AreEqual("f1", r.bms[0].Name);

            r = bReader.FindById(3);
            Assert.AreEqual(2, r.bms.Count);
            Assert.AreEqual("f2", r.bms[0].Name);
            Assert.AreEqual("f4", r.bms[1].Name);
        }

        [Test]
        public void Test3()
        {
            BelongsMore b = BelongsMore.FindById(1);
            Assert.IsNotNull(b);
            Assert.AreEqual("The lovely bones", b.art.Name);
            Assert.AreEqual("jerry", b.rd.Name);

            b = BelongsMore.FindById(3);
            Assert.AreEqual("The load of rings", b.art.Name);
            Assert.AreEqual("tom", b.rd.Name);
        }

        [Test]
        public void TestInsert()
        {
            var r = new bReader {Name = "test"};
            r.Save();

            bReader r1 = bReader.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(0, r1.bms.Count);
        }

        [Test]
        public void TestInsertRelations()
        {
            var r = new bReader {Name = "test"};
            var b = new BelongsMore {Name = "b"};
            r.bms.Add(b);
            r.Save();

            bReader r1 = bReader.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(1, r1.bms.Count);
            Assert.AreEqual("b", r1.bms[0].Name);

            Assert.AreEqual("test", r1.bms[0].rd.Name);
            Assert.IsNull(r1.bms[0].art);
        }

        [Test]
        public void TestInsertRelations2()
        {
            var r = new bReader {Name = "test"};
            var a = new bArticle {Name = "art"};
            var b = new BelongsMore {Name = "b"};
            r.bms.Add(b);
            a.bms.Add(b);
            r.Save();
            //a.Save();

            bReader r1 = bReader.FindById(r.Id);
            Assert.AreEqual("test", r1.Name);
            Assert.AreEqual(1, r1.bms.Count);
            Assert.AreEqual("b", r1.bms[0].Name);
            Assert.AreEqual("test", r1.bms[0].rd.Name);
            Assert.AreEqual("art", r1.bms[0].art.Name);
        }

        [Test]
        public void TestUpdate()
        {
            bArticle a = bArticle.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("f1", a.bms[0].Name);

            a.Name = "haha";
            a.bms[0].Name = "ok";
            a.Save();

            a = bArticle.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("haha", a.Name);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("ok", a.bms[0].Name);
        }

        [Test]
        public void TestUpdate2()
        {
            bArticle a = bArticle.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("f1", a.bms[0].Name);

            a.bms[0].Name = "ok";
            a.Save();

            a = bArticle.FindById(1);
            Assert.IsNotNull(a);
            Assert.AreEqual("ok", a.bms[0].Name);
        }

        [Test]
        public void TestUpdate3()
        {
            bArticle a1 = bArticle.FindById(1);
            bArticle a2 = bArticle.FindById(2);
            a2.bms.Add(a1.bms[0]);
            a2.Save();

            a1 = bArticle.FindById(1);
            Assert.IsNotNull(a1);
            Assert.AreEqual("The lovely bones", a1.Name);
            Assert.AreEqual(0, a1.bms.Count);

            a2 = bArticle.FindById(2);
            Assert.AreEqual(2, a2.bms.Count);
            Assert.AreEqual("f1", a2.bms[0].Name);
            Assert.AreEqual("f2", a2.bms[1].Name);
        }

        [Test]
        public void TestDelete()
        {
            bArticle.FindById(1).Delete();

            bReader r = bReader.FindById(2);
            Assert.AreEqual("f1", r.bms[0].Name);
            Assert.IsNull(r.bms[0].art);
        }

        [Test]
        public void TestDelete2()
        {
            bArticle.FindById(1).Delete();
            // it's don't need to be.
            // Assert.AreEqual(0, DbEntry.Provider.ExecuteScalar("select [Article_Id] from [BelongsMore] Where [Id] = 1"));
        }

        [Test]
        public void TestDelete3()
        {
            bArticle a = bArticle.FindById(1);
            Assert.AreEqual("f1", a.bms[0].Name);
            a.Delete();

            bReader r = bReader.FindById(2);
            Assert.AreEqual(0, r.bms.Count);
        }

        [Test]
        public void TestCreateTable()
        {
            DbEntry.DropAndCreate(typeof(BelongsMore));
            bArticle a = bArticle.FindById(1);
            Assert.AreEqual(0, a.bms.Count);

            a.bms.Add(new BelongsMore {Name = "mytest"});
            a.Save();

            a = bArticle.FindById(1);
            Assert.AreEqual(1, a.bms.Count);
            Assert.AreEqual("mytest", a.bms[0].Name);
        }
    }
}
