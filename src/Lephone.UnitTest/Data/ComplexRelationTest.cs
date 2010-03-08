using System.Collections.Generic;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ComplexRelationTest
    {
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
            Assert.AreEqual("c31", c.Cls2.Cls3List[0].Name);
            Assert.AreEqual("c32", c.Cls2.Cls3List[1].Name);
            Assert.AreEqual("c4", c.Cls2.Cls3List[0].Cls4.Name);
            Assert.AreEqual("c5", c.Cls2.Cls3List[0].Cls4.Cls5.Name);
        }
    }
}
