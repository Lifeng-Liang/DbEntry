using System;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    #region objects

    [Serializable]
    public class TableC : DbObjectModel<TableC>
    {
        public string Title { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public IList<TableD> TD { get; private set; }
    }

    [Serializable]
    public class TableD : DbObjectModel<TableD>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public IList<TableC> TC { get; private set; }
    }

    #endregion

    [TestFixture]
    public class HasAndBelongsToManyAssociateTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            // A.Select will load A, A.B will LazyLoading
            var a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            Assert.AreEqual(3, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("jerry", a.Readers[1].Name);
            Assert.AreEqual("mike", a.Readers[2].Name);
        }

        [Test]
        public void Test2()
        {
            // A.Select will load A, if A.B have been modified, then will not Loading B
            var a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            a.Readers.Add(new Reader {Name = "ruby"});
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("ruby", a.Readers[0].Name);
        }

        [Test]
        public void Test3()
        {
            // A.Save will load A, if there is new element in A.B, then insert B, insert A_B
            var a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            a.Readers.Add(new Reader {Name = "ruby"});
            DbEntry.Save(a);
            var a1 = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            Assert.AreEqual(4, a1.Readers.Count);
            Assert.AreEqual("ruby", a1.Readers[3].Name);
        }

        [Test]
        public void Test4()
        {
            // A.Save will save A, if there is loaded element in A.B, then update B, it will not modify A_B
            var a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            a.Readers[0].Name = "eric";

            DbEntry.Save(a);

            a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("eric", a.Readers[0].Name);
        }

        [Test]
        public void Test5()
        {
            // A.Delete will delete A, and delete all the related items in A_B
            var a = DbEntry.GetObject<Article>(1);
            DbEntry.Delete(a);

            a = DbEntry.GetObject<Article>(1);
            Assert.IsNull(a);

            List<Article_Reader> ar = DbEntry.From<Article_Reader>().Where(CK.K["Article_Id"] == 1).Select();
            Assert.AreEqual(0, ar.Count);
        }

        [Test]
        public void Test6()
        {
            // if A doing Insert, A.Save will save A, if there is new element in A.B, then insert B, insert A_B
            var a = new Article {Name = "Call from hell"};
            a.Readers.Add(new Reader {Name = "ruby"});
            DbEntry.Save(a);
            var a1 = DbEntry.GetObject<Article>(a.Id);
            Assert.IsNotNull(a);
            Assert.AreEqual("Call from hell", a.Name);
            Assert.AreEqual(1, a1.Readers.Count);
            Assert.AreEqual("ruby", a1.Readers[0].Name);
        }

        [Test]
        public void Test7()
        {
            // A.Save, if A.B is a loaded item but insert into A this time, insert A_B
            var a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);

            var r = DbEntry.GetObject<Reader>(2);
            Assert.IsNotNull(r);
            Assert.AreEqual("jerry", r.Name);

            a.Readers.Add(r);

            DbEntry.Save(a);

            a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(2, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("jerry", a.Readers[1].Name);
        }

        [Test]
        public void Test8()
        {
            // A.Save, if A.B is a loaded item but insert into A this time, insert A_B
            var a = DbEntry.GetObject<Article>(3);
            var r = DbEntry.GetObject<Reader>(2);
            a.Readers.Add(r);
            DbEntry.Save(a);

            a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(2, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("jerry", a.Readers[1].Name);
        }

        [Test]
        public void Test9()
        {
            // A.Save, if A.B is loaded item and remove it from A, delete A_B
            var a = DbEntry.GetObject<Article>(1);
            Assert.AreEqual(3, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("jerry", a.Readers[1].Name);
            Assert.AreEqual("mike", a.Readers[2].Name);

            a.Readers.RemoveAt(1);
            Assert.AreEqual(2, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("mike", a.Readers[1].Name);

            DbEntry.Save(a);

            a = DbEntry.GetObject<Article>(1);
            Assert.AreEqual(2, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("mike", a.Readers[1].Name);

            var r = DbEntry.GetObject<Reader>(2);
            Assert.IsNotNull(r);
            Assert.AreEqual("jerry", r.Name);
            Assert.AreEqual(1, r.Articles.Count);
            Assert.AreEqual("The world is float", r.Articles[0].Name);
        }

        [Test]
        public void Test9_1()
        {
            Reader r = Reader.FindById(1);
            Assert.AreEqual(2, r.Articles.Count);
            Assert.AreEqual("The lovely bones", r.Articles[0].Name);
            Assert.AreEqual("The load of rings", r.Articles[1].Name);

            r.Articles.RemoveAt(1);
            Assert.AreEqual(1, r.Articles.Count);
            Assert.AreEqual("The lovely bones", r.Articles[0].Name);

            r.Save();

            r = Reader.FindById(1);
            Assert.AreEqual(1, r.Articles.Count);
            Assert.AreEqual("The lovely bones", r.Articles[0].Name);

            Article a = Article.FindById(3);
            Assert.AreEqual(0, a.Readers.Count);
        }

        [Test]
        public void Test10()
        {
            DbEntry.DropAndCreate(typeof(TableC));
            DbEntry.DropAndCreate(typeof(TableD));
            DbEntry.CreateCrossTable(typeof(TableC), typeof(TableD));

            var t1 = new TableC {Title = "Article1"};
            t1.Save();

            var t3 = new TableD {Name = "Tag1"};
            t3.Save();

            var t2 = TableC.FindOne(p => p.Id == 1);
            t2.TD.Add(t3);
            t2.Save();

            //Begin Remove

            var t4 = TableC.FindById(1);
            Assert.AreEqual(1, t4.TD.Count);
            var t5 = TableD.FindById(1);
            Assert.AreEqual(1, t5.TC.Count);

            bool b = t4.TD.Remove(t5);
            Assert.IsTrue(b);
            t4.Save();

            //here b= false and can't trace the delete sql

            bool b2 = t5.TC.Remove(t4);
            Assert.IsTrue(b2);
            t5.Save();

            //here b2= false and can't trace the delete sql
        }

        [Test, Ignore("for now")]
        public void TestDelteWillRemoveRelation()
        {
            // B.Delete() will cut the relation of it from A
            var a = DbEntry.GetObject<Article>(1);
            Assert.AreEqual(3, a.Readers.Count);
            a.Readers[0].Delete();
            a.Save();

            a = DbEntry.GetObject<Article>(1);
            Assert.AreEqual(2, a.Readers.Count);
        }
    }
}
