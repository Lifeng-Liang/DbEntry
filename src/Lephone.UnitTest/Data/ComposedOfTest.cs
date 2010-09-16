using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public interface ILocation
    {
        string Phone { get; set; }
        string Address { get; set; }
    }

    [TestFixture]
    public class ComposedOfTest : DataTestBase
    {
        public class CoUser : DbObjectModel<CoUser>
        {
            public string Name { get; set; }

            //[ComposedOf]
            [Exclude]
            public ILocation Location { get; private set; }
        }

        [Test]
        public void Test1()
        {
            var user = new CoUser { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street" } };
            user.Save();

            user = CoUser.FindById(user.Id);
            Assert.IsNotNull(user);
            Assert.AreEqual("tom", user.Name);
            Assert.AreEqual("123456", user.Location.Phone);
            Assert.AreEqual("The east of queen street", user.Location.Address);
        }
    }
}
