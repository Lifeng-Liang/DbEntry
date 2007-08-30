
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;

namespace org.hanzify.llf.UnitTest.Data
{
    #region objects

    [DbTable("Article")]
    public abstract class bArticle : DbObjectModel<bArticle>
    {
        public abstract string Name { get; set; }
        [HasMany(OrderBy = "Id")]
        public abstract IList<BelongsMore> bms { get; set; }
    }

    [DbTable("Reader")]
    public abstract class bReader : DbObjectModel<bReader>
    {
        public abstract string Name { get; set; }
        [HasMany(OrderBy = "Id")]
        public abstract IList<BelongsMore> bms { get; set; }
    }

    public abstract class BelongsMore : DbObjectModel<BelongsMore>
    {
        public abstract string Name { get; set; }
        [BelongsTo]
        public abstract bArticle art { get; set; }
        [BelongsTo]
        public abstract bReader rd { get; set; }
    }

    #endregion

    [TestFixture]
    public class BelongsMoreTest
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
    }
}
