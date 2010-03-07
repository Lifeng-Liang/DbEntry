using Lephone.Data;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class HasOneAssociateTest : DataTestBase
    {
        [Test]
        public void TestHasOne1()
        {
            // A.Select will read B (LazyLoading*)
            var p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.PC.Value);

            Assert.AreEqual("Jerry", p.Name);
            Assert.AreEqual("IBM", p.PC.Value.Name);
            Assert.AreEqual(1, p.PC.Value.Id);
        }

        [Test]
        public void TestHasOne1_1()
        {
            // A.Select will read B (LazyLoading*), and set B.a as a
            var p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.PC.Value);
            Assert.AreEqual("Jerry", p.Name);
            p.Name = "Test";
            Assert.AreEqual("Test", p.PC.Value.Owner.Value.Name);
        }

        [Test]
        public void TestHasOne2()
        {
            // A owns 0 or 1 B, so b(b.Value) could be null;
            var Tom = DbEntry.GetObject<Person>(1);
            var Jerry = DbEntry.GetObject<Person>(2);
            var Mike = DbEntry.GetObject<Person>(3);

            Assert.IsNotNull(Tom);
            Assert.IsNotNull(Jerry);
            Assert.IsNotNull(Mike);

            Assert.AreEqual("Tom", Tom.Name);
            Assert.AreEqual("Jerry", Jerry.Name);
            Assert.AreEqual("Mike", Mike.Name);

            Assert.IsNull(Tom.PC.Value);
            Assert.IsNotNull(Jerry.PC.Value);
            Assert.IsNotNull(Mike.PC.Value);

            Assert.AreEqual("IBM", Jerry.PC.Value.Name);
            Assert.AreEqual("DELL", Mike.PC.Value.Name);
        }

        [Test]
        public void TestHasOne3()
        {
            // A.b = new B() will set B.a = A;
            var p = new Person {Name = "NewPerson"};
            var pc = new PersonalComputer {Name = "NewPC"};
            p.PC.Value = pc;
            Assert.AreEqual(pc.Owner.Value, p);
        }

        [Test]
        public void TestHasOne4_1()
        {
            // A.Save will save B, if A is Update, then save B
            var p = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p);
            Assert.IsNull(p.PC.Value);
            p.PC.Value = new PersonalComputer {Name = "NewPC"};
            Assert.AreEqual(0, p.PC.Value.Id);
            DbEntry.Save(p);
            Assert.IsTrue(1 == p.Id);
            Assert.IsTrue(0 != p.PC.Value.Id);

            var p1 = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p1.PC.Value);
            Assert.AreEqual("NewPC", p1.PC.Value.Name);
        }

        [Test]
        public void TestHasOne4_2()
        {
            // A.Save will save B, if A is Insert, then save A first, and then set B.A_id, and save B
            var p = new Person();
            p.Name = "NewPerson";
            p.PC.Value = new PersonalComputer();
            p.PC.Value.Name = "NewPC";
            DbEntry.Save(p);
            Assert.IsTrue(0 != p.Id);
            Assert.IsTrue(0 != p.PC.Value.Id);
            Assert.IsTrue(0 != (long)p.PC.Value.Owner.ForeignKey);

            var p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.AreEqual("NewPerson", p1.Name);
            Assert.IsNotNull(p1.PC.Value);
            Assert.AreEqual("NewPC", p1.PC.Value.Name);
        }

        [Test]
        public void TestHasOne5()
        {
            // A.Delete will delete itself, and delete B *
            var p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.PC.Value);
            long pcid = p.PC.Value.Id;
            // do delete
            DbEntry.Delete(p);
            var p1 = DbEntry.GetObject<Person>(2);
            Assert.IsNull(p1);
            var pc = DbEntry.GetObject<PersonalComputer>(pcid);
            Assert.IsNull(pc);
        }

        [Test]
        public void TestHasOne6()
        {
            // B has a foreign key  A_id
            // B.a = A will set the value of B.A_id
            // B.a = A will set A.a = b ????
            var p = DbEntry.GetObject<Person>(3);
            var pc = new PersonalComputer();
            pc.Name = "NewPC";
            pc.Owner.Value = p;

            Assert.AreEqual(pc.Owner.ForeignKey, 3);
        }

        [Test]
        public void TestHasOne7()
        {
            // B.Save will save itself
            var p = new Person {Name = "NewPerson"};
            var pc = new PersonalComputer();
            pc.Name = "NewPC";
            p.PC.Value = pc;

            DbEntry.Save(pc);

            Assert.IsTrue(0 != pc.Id);
            Assert.IsTrue(0 != p.Id);
        }

        [Test]
        public void TestHasOne8()
        {
            // B.Delete will delete itself
            var p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.PC.Value);
            DbEntry.Delete(p.PC.Value);
            p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNull(p.PC.Value);
        }

        [Test]
        public void TestHasOne10()
        {
            // B.Select will read A (LazyLoading*)
            var pc = DbEntry.GetObject<PersonalComputer>(2);
            Assert.IsNotNull(pc);
            Assert.IsNotNull(pc.Owner.Value);
            Assert.AreEqual("Mike", pc.Owner.Value.Name);
        }

        [Test]
        public void TestHasOne11()
        {
            // B.Select will read A (LazyLoading*), and set A.b as B
            var pc = DbEntry.GetObject<PersonalComputer>(2);
            Assert.IsNotNull(pc);
            Assert.IsNotNull(pc.Owner.Value);
            Assert.AreEqual("Mike", pc.Owner.Value.Name);
            pc.Name = "Test";
            Assert.AreEqual("Test", pc.Owner.Value.PC.Value.Name);
        }

        [Test]
        public void TestHasOne12()
        {
            // A.Save will save B, if B(B.Value) is null, then don't save B (if there is data B in the database, it will still there)
            var p = new Person {Name = "Ghost"};
            DbEntry.Save(p);
            var p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.IsNotNull(p1);
            Assert.AreEqual("Ghost", p1.Name);
            Assert.AreEqual(p.Id, p1.Id);
            Assert.AreEqual(null, p1.PC.Value);
        }

        [Test]
        public void TestHasOne13()
        {
            // For readed A, A.Save will save B, if B(B.Value) is null, then don't save B (if there is data B in the database, it will still there)
            var p = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p);
            Assert.IsNull(p.PC.Value);
            DbEntry.Save(p);
            var p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.IsNotNull(p1);
            Assert.AreEqual(p.Name, p1.Name);
            Assert.AreEqual(p.Id, p1.Id);
            Assert.AreEqual(null, p1.PC.Value);
        }

        [Test]
        public void TestRemoveRelation()
        {
            var Jerry = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(Jerry);
            Assert.AreEqual("Jerry", Jerry.Name);
            Assert.IsNotNull(Jerry.PC.Value);
            Assert.AreEqual("IBM", Jerry.PC.Value.Name);

            Jerry.PC.Value = null;
            DbEntry.Save(Jerry);

            Jerry = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(Jerry);
            Assert.IsNull(Jerry.PC.Value);
        }

        [Test]
        public void TestRemoveRelation2()
        {
            var pc = DbEntry.GetObject<PersonalComputer>(1);
            Assert.IsNotNull(pc);
            Assert.IsNotNull(pc.Owner.Value);

            pc.Owner.Value = null;
            DbEntry.Save(pc);

            pc = DbEntry.GetObject<PersonalComputer>(1);
            Assert.IsNotNull(pc);
            Assert.IsNull(pc.Owner.Value);
        }
    }
}
