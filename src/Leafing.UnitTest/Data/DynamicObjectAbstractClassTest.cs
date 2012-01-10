using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class DynamicObjectAbstractClassTest
    {
        [Test]
        public void TestCreateAbstractClass()
        {
            var c = new AbstractClass();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            Assert.AreEqual("Tom", c.Name);
        }

        [Test]
        public void TestCreateInheritedAbstractClass()
        {
            var c = new AbstractClassOfAge();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            c.Age = 18;
            Assert.AreEqual("Tom", c.Name);
            Assert.AreEqual(18, c.Age);
        }

        [Test]
        public void TestCreateInheritsedAbstractClassWithImplProperty()
        {
            var c = new AbstractClassWithOneImplProperty();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            c.Age = 18;
            Assert.AreEqual("Tom", c.Name);
            Assert.AreEqual(18, c.Age);

            c.Gender = true;
            Assert.AreEqual(false, c.Gender);
        }
    }
}
