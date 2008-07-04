﻿using System.Collections.Generic;
using Lephone.Data;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class HasAndBelongsToManyAssociateTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void Test1()
        {
            // A.Select 将会载入 A, A.B 将会 LazyLoading
            Article a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            Assert.AreEqual(3, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);
            Assert.AreEqual("jerry", a.Readers[1].Name);
            Assert.AreEqual("mike", a.Readers[2].Name);
        }

        [Test]
        public void Test2()
        {
            // A.Select 将会载入 A, 如果 A.B 被修改，则不再 Loading B
            Article a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            a.Readers.Add(Reader.New().Init("ruby"));
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("ruby", a.Readers[0].Name);
        }

        [Test]
        public void Test3()
        {
            // A.Save 将会保存 A, 如果 A.B 中有新元素，则插入 B，插入 A_B
            Article a = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            a.Readers.Add(Reader.New().Init("ruby"));
            DbEntry.Save(a);
            Article a1 = DbEntry.GetObject<Article>(1);
            Assert.IsNotNull(a);
            Assert.AreEqual(4, a1.Readers.Count);
            Assert.AreEqual("ruby", a1.Readers[3].Name);
        }

        [Test]
        public void Test4()
        {
            // A.Save 将会保存 A, 如果 A.B 中有载入的元素，则 update B，不修改 A_B
            Article a = DbEntry.GetObject<Article>(3);
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
            // A.Delete 将会删除 A， 并且删除 A_B 中所有和 A 相关的条目
            Article a = DbEntry.GetObject<Article>(1);
            DbEntry.Delete(a);

            a = DbEntry.GetObject<Article>(1);
            Assert.IsNull(a);

            List<Article_Reader> ar = DbEntry.From<Article_Reader>().Where(CK.K["Article_Id"] == 1).Select();
            Assert.AreEqual(0, ar.Count);
        }

        [Test]
        public void Test6()
        {
            // 如果 A 为 Insert, A.Save 将会保存 A, 如果 A.B 中有新元素，则插入 B，插入 A_B
            Article a = Article.New().Init("Call from hell");
            a.Readers.Add(Reader.New().Init("ruby"));
            DbEntry.Save(a);
            Article a1 = DbEntry.GetObject<Article>(a.Id);
            Assert.IsNotNull(a);
            Assert.AreEqual("Call from hell", a.Name);
            Assert.AreEqual(1, a1.Readers.Count);
            Assert.AreEqual("ruby", a1.Readers[0].Name);
        }

        [Test]
        public void Test7()
        {
            // A.Save, if A.B is a loaded item but insert into A this time, insert A_B
            Article a = DbEntry.GetObject<Article>(3);
            Assert.IsNotNull(a);
            Assert.AreEqual(1, a.Readers.Count);
            Assert.AreEqual("tom", a.Readers[0].Name);

            Reader r = DbEntry.GetObject<Reader>(2);
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
            Article a = DbEntry.GetObject<Article>(3);
            Reader r = DbEntry.GetObject<Reader>(2);
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
            Article a = DbEntry.GetObject<Article>(1);
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

            Reader r = DbEntry.GetObject<Reader>(2);
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
    }
}
