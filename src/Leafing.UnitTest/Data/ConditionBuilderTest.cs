using Leafing.Data;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class ConditionBuilderTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var builder = new ConditionBuilder<SinglePerson>();
            builder &= p => p.Id == 2;
            var condition = builder.ToCondition();
            var list = DbEntry.From<SinglePerson>().Where(condition).Select();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Jerry", list[0].Name);
        }

        [Test]
        public void Test2()
        {
            var builder = new ConditionBuilder<SinglePerson>();
            builder &= p => p.Id == 1;
            builder |= p => p.Id == 3;
            var condition = builder.ToCondition();
            var list = DbEntry.From<SinglePerson>().Where(condition).OrderBy(p => p.Id).Select();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Tom", list[0].Name);
            Assert.AreEqual("Mike", list[1].Name);
        }

        [Test]
        public void Test3()
        {
            var builder = new ConditionBuilder<SinglePerson>();
            builder &= p => p.Id == 1;
            builder |= p => p.Id == 3;
            var list = DbEntry.From<SinglePerson>().Where(builder).OrderBy(p => p.Id).Select();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Tom", list[0].Name);
            Assert.AreEqual("Mike", list[1].Name);
        }

        [Test]
        public void Test4()
        {
            var builder = new ConditionBuilder<SinglePerson2>(p => p.Id == 1);
            builder |= p => p.Id == 3;
            var list = SinglePerson2.Find(builder, p => p.Id);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Tom", list[0].Name);
            Assert.AreEqual("Mike", list[1].Name);
        }

        [Test]
        public void Test5()
        {
            var builder = new ConditionBuilder<SinglePerson2>(p => p.Id == 1);
            builder |= p => p.Id == 3;
            var user = SinglePerson2.FindOne(builder, p => p.Id);
            Assert.AreEqual("Tom", user.Name);
        }
    }
}
