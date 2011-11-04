using System.Collections.Generic;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public class OverPeople : DbObjectModel<OverPeople>
    {
        public string Name { get; set; }

        [HasMany]
        public IList<OverPc> Pcs { get; private set; }

        [Exclude] public int SaveCount;

        protected override void OnInserting()
        {
            SaveCount++;
        }

        protected override void OnUpdating()
        {
            SaveCount++;
        }
    }

    public class OverPc : DbObjectModel<OverPc>
    {
        public string Name { get; set; }

        [BelongsTo]
        public OverPeople Person { get; set; }

        [Exclude] public int SaveCount;

        protected override void OnInserting()
        {
            SaveCount++;
        }

        protected override void OnUpdating()
        {
            SaveCount++;
        }
    }

    [TestFixture]
    public class ModelOverrideSaveTest : SqlTestBase
    {
        [Test]
        public void Test1()
        {
            var p = new OverPeople {Name = "ttt"};
            var c = new OverPc {Name = "ii"};
            var d = new OverPc {Name = "oo"};
            p.Pcs.Add(c);
            p.Pcs.Add(d);
            p.Save();
            Assert.AreEqual(1, p.SaveCount);
            Assert.AreEqual(1, c.SaveCount);
            Assert.AreEqual(1, d.SaveCount);
        }

        [Test]
        public void Test2()
        {
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
