using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data.Inner
{
    [TestFixture]
    public class PreComposedOfTest : DataTestBase
    {
        public interface ILocation
        {
            string Phone { get; set; }
            string Address { get; set; }
            int Number { get; set; }
            int? Wow { get; set; }
        }

        public class CoUserLocation : ILocation
        {
            private readonly CoUser1 _owner;

            public CoUserLocation(CoUser1 owner)
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

            public int Number
            {
                get { return _owner.LocationNumber; }
                set { _owner.LocationNumber = value; }
            }

            public int? Wow
            {
                get { return _owner.LocationWow; }
                set { _owner.LocationWow = value; }
            }
        }

        public class CoUser1 : DbObjectModel<CoUser1>
        {
            public string Name { get; set; }

            public string LocationPhone { get; set; }
            public string LocationAddress { get; set; }
            public int LocationNumber { get; set; }
            public int? LocationWow { get; set; }

            private ILocation _location;

            [Exclude]
            public ILocation Location
            {
                get
                {
                    return _location;
                }
            }

            public CoUser1()
            {
                _location = new CoUserLocation(this);
            }
        }

        [Test]
        public void Test1()
        {
            var user = new CoUser1 {Name = "tom", Location = {Phone = "123456", Address = "The east of queen street", Number = 5, Wow = 8}};
            user.Save();

            user = CoUser1.FindById(user.Id);
            Assert.IsNotNull(user);
            Assert.AreEqual("tom", user.Name);
            Assert.AreEqual("123456", user.Location.Phone);
            Assert.AreEqual("The east of queen street", user.Location.Address);
            Assert.AreEqual(5, user.Location.Number);
            Assert.AreEqual(8, user.Location.Wow);
        }

        [Test]
        public void Test2()
        {
            var user = new CoUser1 { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street", Number = 5 } };
            user.Save();

            user = CoUser1.FindById(user.Id);
            user.Location.Address = "test";
            user.Location.Number = 9;
            user.Save();

            user = CoUser1.FindById(user.Id);
            Assert.AreEqual("test", user.Location.Address);
            Assert.AreEqual(9, user.Location.Number);
            Assert.AreEqual(null, user.Location.Wow);
        }
    }
}
