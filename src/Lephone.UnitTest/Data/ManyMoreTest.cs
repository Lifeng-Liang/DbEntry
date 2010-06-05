using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [DbTable("ManyMore")]
    public class ManyMore : DbObjectModel<ManyMore>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany]
        public IList<ManyMore1> m1 { get; set; }

        [HasAndBelongsToMany]
        public IList<ManyMore2> m2 { get; set; }
    }

    [DbTable("ManyMore1")]
    public class ManyMore1 : DbObjectModel<ManyMore1>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany]
        public IList<ManyMore> m { get; set; }
    }

    [DbTable("ManyMore2")]
    public class ManyMore2 : DbObjectModel<ManyMore2>
    {
        public string Name { get; set; }

        [HasAndBelongsToMany]
        public IList<ManyMore> m { get; set; }
    }

    #endregion

    [TestFixture]
    public class ManyMoreTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var o = new ManyMore {Name = "tom"};
            o.Save();
            o = ManyMore.FindById(o.Id);
            Assert.IsNotNull(o);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(0, o.m1.Count);
            Assert.AreEqual(0, o.m2.Count);

            var o1 = new ManyMore1 {Name = "jerry"};
            o1.Save();
            o1 = ManyMore1.FindById(o1.Id);
            Assert.AreEqual("jerry", o1.Name);
            Assert.AreEqual(0, o1.m.Count);

            var o2 = new ManyMore2 {Name = "mike"};
            o2.Save();
            o2 = ManyMore2.FindById(o2.Id);
            Assert.AreEqual("mike", o2.Name);
            Assert.AreEqual(0, o2.m.Count);

            //======================

            o = new ManyMore {Name = "tom"};
            o.Save();
            o = ManyMore.FindById(o.Id);

            o1 = new ManyMore1 {Name = "jerry"};
            o.m1.Add(o1);

            o2 = new ManyMore2 {Name = "mike"};
            o.m2.Add(o2);

            o.Save();

            o = ManyMore.FindById(o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(1, o.m1.Count);
            Assert.AreEqual(1, o.m2.Count);
            Assert.AreEqual("jerry", o.m1[0].Name);
            Assert.AreEqual("mike", o.m2[0].Name);

            var o1a = new ManyMore1 {Name = "allen"};
            o.m1.Add(o1a);

            o.Save();

            o = ManyMore.FindById(o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(2, o.m1.Count);
            Assert.AreEqual(1, o.m2.Count);
            Assert.AreEqual("jerry", o.m1[0].Name);
            Assert.AreEqual("allen", o.m1[1].Name);
            Assert.AreEqual("mike", o.m2[0].Name);

            o1 = ManyMore1.FindById(o1.Id);
            Assert.AreEqual("jerry", o1.Name);
            Assert.AreEqual(1, o1.m.Count);
            Assert.AreEqual("tom", o1.m[0].Name);

            long oid = o.Id;
            o.Delete();

            o = ManyMore.FindById(oid);
            Assert.IsNull(o);

            o1 = ManyMore1.FindById(o1.Id);
            Assert.AreEqual(0, o1.m.Count);

            o2 = ManyMore2.FindById(o2.Id);
            Assert.AreEqual(0, o2.m.Count);

            int n = Convert.ToInt32(DbEntry.Context.ExecuteScalar("select count(*) from [R_ManyMore_ManyMore1]"));
            Assert.AreEqual(0, n);
            n = Convert.ToInt32(DbEntry.Context.ExecuteScalar("select count(*) from [R_ManyMore_ManyMore2]"));
            Assert.AreEqual(0, n);
        }
    }
}
