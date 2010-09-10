using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
    [TestFixture]
    public class PreComposedOfTest : DataTestBase
    {
        public interface ILocation
        {
            string Phone { get; set; }
            string Address { get; set; }
        }

        public class CoUserLocation : ILocation
        {
            private readonly CoUser _owner;

            public CoUserLocation(CoUser owner)
            {
                this._owner = owner;
            }

            public string Phone
            {
                get { return _owner.LocationPhone; }
                set { _owner.LocationPhone = value; }
            }

            public string Address
            {
                get { return _owner.LocationAddress; }
                set { _owner.LocationAddress = value; }
            }
        }

        public class CoUser : DbObjectModel<CoUser>
        {
            public string Name { get; set; }

            protected internal string LocationPhone;
            protected internal string LocationAddress;

            private ILocation _location;

            [Exclude]
            public ILocation Location
            {
                get
                {
                    return _location;
                }
            }

            public CoUser()
            {
                _location = new CoUserLocation(this);
            }
        }

        [Test]
        public void Test1()
        {
            var user = new CoUser {Name = "tom", Location = {Phone = "123456", Address = "The east of queen street"}};
            user.Save();

            user = CoUser.FindById(user.Id);
            Assert.IsNotNull(user);
            Assert.AreEqual("tom", user.Name);
            Assert.AreEqual("123456", user.Location.Phone);
            Assert.AreEqual("The east of queen street", user.Location.Address);
        }
    }
}
