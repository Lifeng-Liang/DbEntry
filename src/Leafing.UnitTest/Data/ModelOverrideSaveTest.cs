using System.Collections.Generic;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    public class OverPeople : DbObjectModel<OverPeople> {
        public string Name { get; set; }

        public HasMany<OverPc> Pcs { get; private set; }

        [Exclude] public int SaveCount;

        protected override void OnInserting() {
            SaveCount++;
        }

        protected override void OnUpdating() {
            SaveCount++;
        }

        public OverPeople() {
            Pcs = new HasMany<OverPc>(this, "Id", "OverPeople_Id");
        }
    }

    public class OverPc : DbObjectModel<OverPc> {
        public string Name { get; set; }

        public BelongsTo<OverPeople, long> Person { get; set; }

        [Exclude] public int SaveCount;

        protected override void OnInserting() {
            SaveCount++;
        }

        protected override void OnUpdating() {
            SaveCount++;
        }

        public OverPc() {
            Person = new BelongsTo<OverPeople, long>(this, "OverPeople_Id");
        }
    }

    [TestFixture]
    public class ModelOverrideSaveTest : SqlTestBase {
        [Test]
        public void Test1() {
            var p = new OverPeople { Name = "ttt" };
            var c = new OverPc { Name = "ii" };
            var d = new OverPc { Name = "oo" };
            p.Pcs.Add(c);
            p.Pcs.Add(d);
            p.Save();
            Assert.AreEqual(1, p.SaveCount);
            Assert.AreEqual(1, c.SaveCount);
            Assert.AreEqual(1, d.SaveCount);
        }

        [Test]
        public void Test2() {
            var p = new OverPeople { Name = "ttt" };
            var c = new OverPc { Name = "ii" };
            var d = new OverPc { Name = "oo" };
            p.Pcs.Add(c);
            p.Pcs.Add(d);
            c.Save();
            Assert.AreEqual(1, p.SaveCount);
            Assert.AreEqual(1, c.SaveCount);
            Assert.AreEqual(1, d.SaveCount);
        }
    }
}