using System;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    #region objects

    [DbTable("ManyMore")]
    public class ManyMore : DbObjectModel<ManyMore> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMore1> m1 { get; private set; }

        public HasAndBelongsToMany<ManyMore2> m2 { get; private set; }

        public ManyMore() {
            m1 = new HasAndBelongsToMany<ManyMore1>(this, "Id", "ManyMore_Id");
            m2 = new HasAndBelongsToMany<ManyMore2>(this, "Id", "ManyMore_Id");
        }
    }

    [DbTable("ManyMore1")]
    public class ManyMore1 : DbObjectModel<ManyMore1> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMore> m { get; private set; }

        public ManyMore1() {
            m = new HasAndBelongsToMany<ManyMore>(this, "Id", "ManyMore1_Id");
        }
    }

    [DbTable("ManyMore2")]
    public class ManyMore2 : DbObjectModel<ManyMore2> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMore> m { get; private set; }

        public ManyMore2() {
            m = new HasAndBelongsToMany<ManyMore>(this, "Id", "ManyMore2_Id");
        }
    }

    [DbTable("ManyMore"), DbContext("SQLite")]
    public class ManyMoreSqlite : DbObjectModel<ManyMoreSqlite> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMore1Sqlite> m1 { get; private set; }

        public HasAndBelongsToMany<ManyMore2Sqlite> m2 { get; private set; }

        public ManyMoreSqlite() {
            m1 = new HasAndBelongsToMany<ManyMore1Sqlite>(this, "Id", "ManyMore_Id");
            m2 = new HasAndBelongsToMany<ManyMore2Sqlite>(this, "Id", "ManyMore_Id");
        }
    }

    [DbTable("ManyMore1"), DbContext("SQLite")]
    public class ManyMore1Sqlite : DbObjectModel<ManyMore1Sqlite> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMoreSqlite> m { get; private set; }

        public ManyMore1Sqlite() {
            m = new HasAndBelongsToMany<ManyMoreSqlite>(this, "Id", "ManyMore1_Id");
        }
    }

    [DbTable("ManyMore2"), DbContext("SQLite")]
    public class ManyMore2Sqlite : DbObjectModel<ManyMore2Sqlite> {
        public string Name { get; set; }

        public HasAndBelongsToMany<ManyMoreSqlite> m { get; private set; }

        public ManyMore2Sqlite() {
            m = new HasAndBelongsToMany<ManyMoreSqlite>(this, "Id", "ManyMore2_Id");
        }
    }

    #endregion

    [TestFixture]
    public class ManyMoreTest : DataTestBase {
        [Test]
        public void Test1() {
            var o = new ManyMore { Name = "tom" };
            o.Save();
            o = ManyMore.FindById(o.Id);
            Assert.IsNotNull(o);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(0, o.m1.Count);
            Assert.AreEqual(0, o.m2.Count);

            var o1 = new ManyMore1 { Name = "jerry" };
            o1.Save();
            o1 = ManyMore1.FindById(o1.Id);
            Assert.AreEqual("jerry", o1.Name);
            Assert.AreEqual(0, o1.m.Count);

            var o2 = new ManyMore2 { Name = "mike" };
            o2.Save();
            o2 = ManyMore2.FindById(o2.Id);
            Assert.AreEqual("mike", o2.Name);
            Assert.AreEqual(0, o2.m.Count);

            //======================

            o = new ManyMore { Name = "tom" };
            o.Save();
            o = ManyMore.FindById(o.Id);

            o1 = new ManyMore1 { Name = "jerry" };
            o.m1.Add(o1);

            o2 = new ManyMore2 { Name = "mike" };
            o.m2.Add(o2);

            o.Save();

            o = ManyMore.FindById(o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(1, o.m1.Count);
            Assert.AreEqual(1, o.m2.Count);
            Assert.AreEqual("jerry", o.m1[0].Name);
            Assert.AreEqual("mike", o.m2[0].Name);

            var o1a = new ManyMore1 { Name = "allen" };
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

            var oid = o.Id;
            o.Delete();

            o = ManyMore.FindById(oid);
            Assert.IsNull(o);

            o1 = ManyMore1.FindById(o1.Id);
            Assert.AreEqual(0, o1.m.Count);

            o2 = ManyMore2.FindById(o2.Id);
            Assert.AreEqual(0, o2.m.Count);

            int n = Convert.ToInt32(DbEntry.Provider.ExecuteScalar("select count(*) from [R_ManyMore_ManyMore1]"));
            Assert.AreEqual(0, n);
            n = Convert.ToInt32(DbEntry.Provider.ExecuteScalar("select count(*) from [R_ManyMore_ManyMore2]"));
            Assert.AreEqual(0, n);
        }
    }
}