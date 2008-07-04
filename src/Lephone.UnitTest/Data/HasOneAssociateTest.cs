using Lephone.Data;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class HasOneAssociateTest
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
        public void TestHasOne1()
        {
            // A.Select will read B (LazyLoading*)
            Person p = DbEntry.GetObject<Person>(2);
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
            Person p = DbEntry.GetObject<Person>(2);
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
            Person Tom = DbEntry.GetObject<Person>(1);
            Person Jerry = DbEntry.GetObject<Person>(2);
            Person Mike = DbEntry.GetObject<Person>(3);

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
            Person p = new Person();
            p.Name = "NewPerson";
            PersonalComputer pc = new PersonalComputer();
            pc.Name = "NewPC";
            p.PC.Value = pc;
            Assert.AreEqual(pc.Owner.Value, p);
        }

        [Test]
        public void TestHasOne4_1()
        {
            // A.Save will save B, if A is Update, then save B
            Person p = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p);
            Assert.IsNull(p.PC.Value);
            p.PC.Value = new PersonalComputer();
            p.PC.Value.Name = "NewPC";
            Assert.AreEqual(0, p.PC.Value.Id);
            DbEntry.Save(p);
            Assert.IsTrue(1 == p.Id);
            Assert.IsTrue(0 != p.PC.Value.Id);

            Person p1 = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p1.PC.Value);
            Assert.AreEqual("NewPC", p1.PC.Value.Name);
        }

        [Test]
        public void TestHasOne4_2()
        {
            // A.Save will save B, if A is Insert, then save A first, and then set B.A_id, and save B
            Person p = new Person();
            p.Name = "NewPerson";
            p.PC.Value = new PersonalComputer();
            p.PC.Value.Name = "NewPC";
            DbEntry.Save(p);
            Assert.IsTrue(0 != p.Id);
            Assert.IsTrue(0 != p.PC.Value.Id);
            Assert.IsTrue(0 != (long)p.PC.Value.Owner.ForeignKey);

            Person p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.AreEqual("NewPerson", p1.Name);
            Assert.IsNotNull(p1.PC.Value);
            Assert.AreEqual("NewPC", p1.PC.Value.Name);
        }

        [Test]
        public void TestHasOne5()
        {
            // A.Delete will delete itself, and delete B *
            Person p = DbEntry.GetObject<Person>(2);
            Assert.IsNotNull(p);
            Assert.IsNotNull(p.PC.Value);
            long pcid = p.PC.Value.Id;
            // do delete
            DbEntry.Delete(p);
            Person p1 = DbEntry.GetObject<Person>(2);
            Assert.IsNull(p1);
            PersonalComputer pc = DbEntry.GetObject<PersonalComputer>(pcid);
            Assert.IsNull(pc);
        }

        [Test]
        public void TestHasOne6()
        {
            // B has a foreign key  A_id
            // B.a = A will set the value of B.A_id
            // B.a = A will set A.a = b ????
            Person p = DbEntry.GetObject<Person>(3);
            PersonalComputer pc = new PersonalComputer();
            pc.Name = "NewPC";
            pc.Owner.Value = p;

            Assert.AreEqual(pc.Owner.ForeignKey, 3);
        }

        [Test]
        public void TestHasOne7()
        {
            // B.Save will save itself
            Person p = new Person();
            p.Name = "NewPerson";
            PersonalComputer pc = new PersonalComputer();
            pc.Name = "NewPC";
            p.PC.Value = pc;

            DbEntry.Save(pc);

            Assert.IsTrue(0 != pc.Id);
            Assert.IsTrue(0 == p.Id);
        }

        [Test]
        public void TestHasOne8()
        {
            // B.Delete will delete itself
            Person p = DbEntry.GetObject<Person>(2);
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
            PersonalComputer pc = DbEntry.GetObject<PersonalComputer>(2);
            Assert.IsNotNull(pc);
            Assert.IsNotNull(pc.Owner.Value);
            Assert.AreEqual("Mike", pc.Owner.Value.Name);
        }

        [Test]
        public void TestHasOne11()
        {
            // B.Select will read A (LazyLoading*), and set A.b as B
            PersonalComputer pc = DbEntry.GetObject<PersonalComputer>(2);
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
            Person p = new Person();
            p.Name = "Ghost";
            DbEntry.Save(p);
            Person p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.IsNotNull(p1);
            Assert.AreEqual("Ghost", p1.Name);
            Assert.AreEqual(p.Id, p1.Id);
            Assert.AreEqual(null, p1.PC.Value);
        }

        [Test]
        public void TestHasOne13()
        {
            // For readed A, A.Save will save B, if B(B.Value) is null, then don't save B (if there is data B in the database, it will still there)
            Person p = DbEntry.GetObject<Person>(1);
            Assert.IsNotNull(p);
            Assert.IsNull(p.PC.Value);
            DbEntry.Save(p);
            Person p1 = DbEntry.GetObject<Person>(p.Id);
            Assert.IsNotNull(p1);
            Assert.AreEqual(p.Name, p1.Name);
            Assert.AreEqual(p.Id, p1.Id);
            Assert.AreEqual(null, p1.PC.Value);
        }

        [Test]
        public void TestRemoveRelation()
        {
            Person Jerry = DbEntry.GetObject<Person>(2);
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
            PersonalComputer pc = DbEntry.GetObject<PersonalComputer>(1);
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
