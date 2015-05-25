using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class ComplexRelationTest : DataTestBase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            DbEntry.DropAndCreate(typeof(Cls1));
            DbEntry.DropAndCreate(typeof(Cls2));
            DbEntry.DropAndCreate(typeof(Cls3));
            DbEntry.DropAndCreate(typeof(Cls4));
            DbEntry.DropAndCreate(typeof(Cls5));
            DbEntry.DropAndCreate(typeof(OverSave));
            DbEntry.DropAndCreate(typeof(OverSave2));
            DbEntry.DropAndCreate(typeof(OverSave3));
            DbEntry.DropAndCreate(typeof(OverSave4));
        }

        public class Cls1 : DbObjectModel<Cls1>
        {
            public string Name { get; set; }
            [HasOne]
            public Cls2 Cls2 { get; set; }
        }

        public class Cls2 : DbObjectModel<Cls2>
        {
            public string Name { get; set; }
            [BelongsTo]
            public Cls1 Cls1 { get; set; }
            [HasMany(OrderBy = "Id")]
            public IList<Cls3> Cls3List { get; private set; }
        }

        public class Cls3 : DbObjectModel<Cls3>
        {
            public string Name { get; set; }
            [BelongsTo]
            public Cls2 Cls2 { get; set; }
            [BelongsTo]
            public Cls4 Cls4 { get; set; }
        }

        public class Cls4 : DbObjectModel<Cls4>
        {
            public string Name { get; set; }
            [HasMany(OrderBy = "Id")]
            public IList<Cls3> Cls3List { get; private set; }
            [BelongsTo]
            public Cls5 Cls5 { get; set; }
        }

        public class Cls5 : DbObjectModel<Cls5>
        {
            public string Name { get; set; }
            [HasOne]
            public Cls4 Cls4 { get; set; }
        }

        public class OverSave : DbObjectModel<OverSave>
        {
            public string Name { get; set; }

            [HasMany]
            public IList<OverSave2> Overs { get; private set; }
        }

        public class OverSave2 : DbObjectModel<OverSave2>
        {
            public string Name { get; set; }

            [BelongsTo]
            public OverSave Over { get; set; }

            protected override void OnInserting()
            {
                Name += "01";
            }
        }

        public class OverSave3 : DbObjectModel<OverSave3>
        {
            public string Name { get; set; }

            [HasMany]
            public IList<OverSave4> Overs { get; private set; }
        }

        public class OverSave4 : DbObjectModel<OverSave4>
        {
            public string Name { get; set; }

            [BelongsTo]
            public OverSave3 Over { get; set; }

            protected override void OnInserting()
            {
                this.Name = "change";
            }

            protected override void OnUpdating()
            {
                this.Name = "change";
            }
        }

        [Test]
        public void Test1()
        {
            var c1 = new Cls1 {Name = "c1", Cls2 = new Cls2 {Name = "c2"}};
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c31"});
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c32"});
            var c4 = new Cls4 {Name = "c4"};
            c4.Cls3List.Add(c1.Cls2.Cls3List[0]);
            new Cls5 {Name = "c5", Cls4 = c1.Cls2.Cls3List[0].Cls4};
            c1.Save();

            var c = Cls1.FindById(c1.Id);
            Assert.AreEqual("c1", c.Name);
            Assert.AreEqual("c2", c.Cls2.Name);
            Assert.AreEqual(2, c.Cls2.Cls3List.Count);
            Assert.AreEqual("c31", c.Cls2.Cls3List[0].Name);
            Assert.AreEqual("c32", c.Cls2.Cls3List[1].Name);
            Assert.AreEqual("c4", c.Cls2.Cls3List[0].Cls4.Name);
            Assert.AreEqual("c5", c.Cls2.Cls3List[0].Cls4.Cls5.Name);
        }

        [Test]
        public void Test2()
        {
            var c1 = new Cls1 {Name = "c1", Cls2 = new Cls2 {Name = "c2"}};
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c31"});
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c32"});
            c1.Cls2.Cls3List[0].Cls4 = new Cls4 {Name = "c4", Cls5 = new Cls5 {Name = "c5"}};

            c1.Save();

            var c = Cls1.FindById(c1.Id);
            Assert.AreEqual("c1", c.Name);
            Assert.AreEqual("c2", c.Cls2.Name);
            Assert.AreEqual(2, c.Cls2.Cls3List.Count);
            Assert.AreEqual("c31", c.Cls2.Cls3List[0].Name);
            Assert.AreEqual("c32", c.Cls2.Cls3List[1].Name);
            Assert.AreEqual("c4", c.Cls2.Cls3List[0].Cls4.Name);
            Assert.AreEqual("c5", c.Cls2.Cls3List[0].Cls4.Cls5.Name);
        }

        [Test]
        public void Test3()
        {
            var c1 = new Cls1 {Name = "c1", Cls2 = new Cls2 {Name = "c2"}};
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c31"});
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c32"});
            c1.Cls2.Cls3List[0].Cls4 = new Cls4 {Name = "c4", Cls5 = new Cls5 {Name = "c5"}};

            c1.Save();

            var c = Cls1.FindById(c1.Id);
            c.Cls2.Cls3List.Add(new Cls3 {Name = "c33"});

            c.Save();

            c = Cls1.FindById(c1.Id);
            Assert.AreEqual("c1", c.Name);
            Assert.AreEqual("c2", c.Cls2.Name);
            Assert.AreEqual(3, c.Cls2.Cls3List.Count);
            Assert.AreEqual("c31", c.Cls2.Cls3List[0].Name);
            Assert.AreEqual("c32", c.Cls2.Cls3List[1].Name);
            Assert.AreEqual("c33", c.Cls2.Cls3List[2].Name);
            Assert.AreEqual("c4", c.Cls2.Cls3List[0].Cls4.Name);
            Assert.AreEqual("c5", c.Cls2.Cls3List[0].Cls4.Cls5.Name);
        }

        [Test]
        public void Test4()
        {
            var c1 = new Cls1 {Name = "c1", Cls2 = new Cls2 {Name = "c2"}};
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c31"});
            c1.Cls2.Cls3List.Add(new Cls3 {Name = "c32"});
            c1.Cls2.Cls3List[0].Cls4 = new Cls4 {Name = "c4", Cls5 = new Cls5 {Name = "c5"}};

            c1.Save();

            var c4 = Cls4.FindById(c1.Cls2.Cls3List[0].Cls4.Id);
            c4.Cls5 = new Cls5 {Name = "c5x"};

            c4.Save();

            var c = Cls1.FindById(c1.Id);
            Assert.AreEqual("c1", c.Name);
            Assert.AreEqual("c2", c.Cls2.Name);
            Assert.AreEqual(2, c.Cls2.Cls3List.Count);
            Assert.AreEqual("c31", c.Cls2.Cls3List[0].Name);
            Assert.AreEqual("c32", c.Cls2.Cls3List[1].Name);
            Assert.AreEqual("c4", c.Cls2.Cls3List[0].Cls4.Name);
            Assert.AreEqual("c5x", c.Cls2.Cls3List[0].Cls4.Cls5.Name);
        }

        [Test]
        public void TestOverrideSave()
        {
            var o = new OverSave {Name = "test"};
            o.Overs.Add(new OverSave2 {Name = "ok"});
            o.Save();

            var o1 = OverSave.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(1, o1.Overs.Count);
            Assert.AreEqual("ok01", o1.Overs[0].Name);
        }

        [Test]
        public void TestOverrideSave2()
        {
            var o = new OverSave3 {Name = "test"};
            o.Overs.Add(new OverSave4 {Name = "ok"});
            o.Save();

            var o1 = OverSave3.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(1, o1.Overs.Count);
            Assert.AreEqual("change", o1.Overs[0].Name);
        }
    }
}
