using Lephone.Data;
using Lephone.Data.Common;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ComplexDynamicObjectTest : DataTestBase
    {
        [Test]
        public void TestCross()
        {
            var de = new DbContext(EntryConfig.GetDriver("SQLite"));
            de.From<ImpPeople>().Where(WhereCondition.EmptyCondition).Select();
            StaticRecorder.ClearMessages();

            ImpPeople p = ImpPeople.FindById(1);
            p.Save();
            Assert.AreEqual(0, StaticRecorder.Messages.Count);

            p.Name = "abc";
            de.Save(p);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [People] Set [Name]=@Name_0  Where [Id] = @Id_1;\n<Text><30>(@Name_0=abc:String,@Id_1=1:Int64)", StaticRecorder.Messages[0]);
        }

        [Test]
        public void TestImpedHasOne()
        {
            ImpPeople p = ImpPeople.FindById(1);
            Assert.AreEqual("Tom", p.Name);
            Assert.IsNull(p.pc);

            p = ImpPeople.FindById(2);
            Assert.AreEqual("Jerry", p.Name);
            Assert.AreEqual("IBM", p.pc.Name);

            p = ImpPeople.FindById(3);
            Assert.AreEqual("Mike", p.Name);
            Assert.AreEqual("DELL", p.pc.Name);
        }

        [Test]
        public void TestDynamicObjectConstractor()
        {
            PeopleWith p = PeopleWith.FindById(1);
            Assert.IsNotNull(p.GetUpdateColumns());
        }

        [Test]
        public void TestHasOne()
        {
            People p = People.FindById(1);
            Assert.AreEqual("Tom", p.Name);
            Assert.IsNull(p.pc);

            p = People.FindById(2);
            Assert.AreEqual("Jerry", p.Name);
            Assert.AreEqual("IBM", p.pc.Name);

            p = People.FindById(3);
            Assert.AreEqual("Mike", p.Name);
            Assert.AreEqual("DELL", p.pc.Name);

            p = People.FindById(3);
            p.Name = "me";
            p.pc.Name = "test";
            p.Save();

            p = People.FindById(3);
            Assert.AreEqual("me", p.Name);
            Assert.AreEqual("test", p.pc.Name);
        }

        [Test]
        public void TestImpedHasMany()
        {
            ImpPeople1 p = ImpPeople1.FindById(1);
            Assert.AreEqual("Tom", p.Name);
            Assert.AreEqual(0, p.pcs.Count);

            p = ImpPeople1.FindById(2);
            Assert.AreEqual("Jerry", p.Name);
            Assert.AreEqual(1, p.pcs.Count);
            Assert.AreEqual("IBM", p.pcs[0].Name);

            p = ImpPeople1.FindById(3);
            Assert.AreEqual("Mike", p.Name);
            Assert.AreEqual(2, p.pcs.Count);
            Assert.AreEqual("HP", p.pcs[0].Name);
            Assert.AreEqual("DELL", p.pcs[1].Name);
        }

        [Test]
        public void TestHasMany()
        {
            People1 p = People1.FindById(1);
            Assert.AreEqual("Tom", p.Name);
            Assert.AreEqual(0, p.pcs.Count);

            p = People1.FindById(2);
            Assert.AreEqual("Jerry", p.Name);
            Assert.AreEqual(1, p.pcs.Count);
            Assert.AreEqual("IBM", p.pcs[0].Name);

            p = People1.FindById(3);
            Assert.AreEqual("Mike", p.Name);
            Assert.AreEqual(2, p.pcs.Count);
            Assert.AreEqual("HP", p.pcs[0].Name);
            Assert.AreEqual("DELL", p.pcs[1].Name);
        }

        [Test]
        public void TestHasAndBelongsToMany()
        {
            DArticle a = DArticle.FindById(1);
            Assert.AreEqual("The lovely bones", a.Name);
            Assert.AreEqual(3, a.readers.Count);
            Assert.AreEqual("tom", a.readers[0].Name);
            Assert.AreEqual("jerry", a.readers[1].Name);
            Assert.AreEqual("mike", a.readers[2].Name);

            a = DArticle.FindById(2);
            Assert.AreEqual("The world is float", a.Name);
            Assert.AreEqual(2, a.readers.Count);
            Assert.AreEqual("jerry", a.readers[0].Name);
            Assert.AreEqual("mike", a.readers[1].Name);

            a = DArticle.FindById(3);
            Assert.AreEqual("The load of rings", a.Name);
            Assert.AreEqual(1, a.readers.Count);
            Assert.AreEqual("tom", a.readers[0].Name);
        }

        [Test]
        public void TestOrderByAttribute()
        {
            PeopleImp1 p1 = PeopleImp1.FindById(3);
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p1.pc.Value);
            Assert.AreEqual("DELL", p1.pc.Value.Name);

            PeopleImp2 p2 = PeopleImp2.FindById(3);
            Assert.AreEqual("HP", p2.pc.Value.Name);
        }
    }
}
