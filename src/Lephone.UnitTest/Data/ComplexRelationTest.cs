using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ComplexRelationTest : DataTestBase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            DbEntry.Context.DropAndCreate(typeof(Cls1));
            DbEntry.Context.DropAndCreate(typeof(Cls2));
            DbEntry.Context.DropAndCreate(typeof(Cls3));
            DbEntry.Context.DropAndCreate(typeof(Cls4));
            DbEntry.Context.DropAndCreate(typeof(Cls5));
            DbEntry.Context.DropAndCreate(typeof(OverSave));
            DbEntry.Context.DropAndCreate(typeof(OverSave2));
            DbEntry.Context.DropAndCreate(typeof(OverSave3));
            DbEntry.Context.DropAndCreate(typeof(OverSave4));
        }

        public abstract class Cls1 : DbObjectModel<Cls1>
        {
            public abstract string Name { get; set; }
            [HasOne]
            public abstract Cls2 Cls2 { get; set; }

            public abstract Cls1 Init(string name);
        }

        public abstract class Cls2 : DbObjectModel<Cls2>
        {
            public abstract string Name { get; set; }
            [BelongsTo]
            public abstract Cls1 Cls1 { get; set; }
            [HasMany(OrderBy = "Id")]
            public abstract IList<Cls3> Cls3List { get; set; }

            public abstract Cls2 Init(string name);
        }

        public abstract class Cls3 : DbObjectModel<Cls3>
        {
            public abstract string Name { get; set; }
            [BelongsTo]
            public abstract Cls2 Cls2 { get; set; }
            [BelongsTo]
            public abstract Cls4 Cls4 { get; set; }

            public abstract Cls3 Init(string name);
        }

        public abstract class Cls4 : DbObjectModel<Cls4>
        {
            public abstract string Name { get; set; }
            [HasMany(OrderBy = "Id")]
            public abstract IList<Cls3> Cls3List { get; set; }
            [BelongsTo]
            public abstract Cls5 Cls5 { get; set; }

            public abstract Cls4 Init(string name);
        }

        public abstract class Cls5 : DbObjectModel<Cls5>
        {
            public abstract string Name { get; set; }
            [HasOne]
            public abstract Cls4 Cls4 { get; set; }

            public abstract Cls5 Init(string name);
        }

        public abstract class OverSave : DbObjectModel<OverSave>
        {
            public abstract string Name { get; set; }

            [HasMany]
            public abstract IList<OverSave2> Overs { get; set; }

            public abstract OverSave Init(string name);
        }

        public abstract class OverSave2 : DbObjectModel<OverSave2>
        {
            public abstract string Name { get; set; }

            [BelongsTo]
            public abstract OverSave Over { get; set; }

            public abstract OverSave2 Init(string name);

            public override void Save()
            {
            }
        }

        public abstract class OverSave3 : DbObjectModel<OverSave3>
        {
            public abstract string Name { get; set; }

            [HasMany]
            public abstract IList<OverSave4> Overs { get; set; }

            public abstract OverSave3 Init(string name);
        }

        public abstract class OverSave4 : DbObjectModel<OverSave4>
        {
            public abstract string Name { get; set; }

            [BelongsTo]
            public abstract OverSave3 Over { get; set; }

            public abstract OverSave4 Init(string name);

            public override void Save()
            {
                this.Name = "change";
                base.Save();
            }
        }

        [Test]
        public void Test1()
        {
            var c1 = Cls1.New.Init("c1");
            c1.Cls2 = Cls2.New.Init("c2");
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c31"));
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c32"));
            var c4 = Cls4.New.Init("c4");
            c4.Cls3List.Add(c1.Cls2.Cls3List[0]);
            var c5 = Cls5.New.Init("c5");
            c5.Cls4 = c1.Cls2.Cls3List[0].Cls4;
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
            var c1 = Cls1.New.Init("c1");
            c1.Cls2 = Cls2.New.Init("c2");
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c31"));
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c32"));
            c1.Cls2.Cls3List[0].Cls4 = Cls4.New.Init("c4");
            c1.Cls2.Cls3List[0].Cls4.Cls5 = Cls5.New.Init("c5");

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
            var c1 = Cls1.New.Init("c1");
            c1.Cls2 = Cls2.New.Init("c2");
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c31"));
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c32"));
            c1.Cls2.Cls3List[0].Cls4 = Cls4.New.Init("c4");
            c1.Cls2.Cls3List[0].Cls4.Cls5 = Cls5.New.Init("c5");

            c1.Save();

            var c = Cls1.FindById(c1.Id);
            c.Cls2.Cls3List.Add(Cls3.New.Init("c33"));

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
            var c1 = Cls1.New.Init("c1");
            c1.Cls2 = Cls2.New.Init("c2");
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c31"));
            c1.Cls2.Cls3List.Add(Cls3.New.Init("c32"));
            c1.Cls2.Cls3List[0].Cls4 = Cls4.New.Init("c4");
            c1.Cls2.Cls3List[0].Cls4.Cls5 = Cls5.New.Init("c5");

            c1.Save();

            var c4 = Cls4.FindById(c1.Cls2.Cls3List[0].Cls4.Id);
            c4.Cls5 = Cls5.New.Init("c5x");

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
            var o = OverSave.New.Init("test");
            o.Overs.Add(OverSave2.New.Init("ok"));
            o.Save();

            var o1 = OverSave.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(0, o1.Overs.Count);
        }

        [Test]
        public void TestOverrideSave2()
        {
            var o = OverSave3.New.Init("test");
            o.Overs.Add(OverSave4.New.Init("ok"));
            o.Save();

            var o1 = OverSave3.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(1, o1.Overs.Count);
            Assert.AreEqual("change", o1.Overs[0].Name);
        }
    }
}
