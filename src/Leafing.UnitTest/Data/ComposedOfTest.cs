using Leafing.Core;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    public class ILocation {
        public string Phone { get; set; }

        [AllowNull, Length(2, 50)]
        public string Address { get; set; }

        [DbColumn("MyNumber")]
        public int Number { get; set; }

        public int? Wow { get; set; }
    }

    public class IAddress {
        public string City { get; set; }
        public string Street { get; set; }
    }

    [TestFixture, Ignore("temp")]
    public class ComposedOfTest : DataTestBase {
        public class CoUser : DbObjectModel<CoUser> {
            public string Name { get; set; }

            [ComposedOf]
            public ILocation Location { get; private set; }

            public CoUser() {
                Location = new ILocation();
            }
        }

        public class CoAddr : DbObjectModel<CoAddr> {
            public string Name { get; set; }

            [ComposedOf]
            public IAddress MyAddress { get; private set; }

            [ComposedOf]
            public IAddress YourAddress { get; private set; }

            public CoAddr() {
                MyAddress = new IAddress();
                YourAddress = new IAddress();
            }
        }

        [Test]
        public void Test1() {
            var user = new CoUser { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street" } };
            user.Save();

            user = CoUser.FindById(user.Id);
            Assert.IsNotNull(user);
            Assert.AreEqual("tom", user.Name);
            Assert.AreEqual("123456", user.Location.Phone);
            Assert.AreEqual("The east of queen street", user.Location.Address);
        }

        [Test]
        public void Test2() {
            var user = new CoUser { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street" } };
            user.Save();

            user = CoUser.FindById(user.Id);
            user.Location.Address = "test";
            user.Save();

            user = CoUser.FindById(user.Id);
            Assert.AreEqual("test", user.Location.Address);
        }

        [Test]
        public void TestQuery() {
            var user = new CoUser { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street" } };
            user.Save();

            var ru = CoUser.FindOne(p => p.Location.Phone == "123456");
            Assert.AreEqual("The east of queen street", ru.Location.Address);
        }

        [Test]
        public void Test3() {
            var type = typeof(CoUser);
            var addr = type.GetProperty("$Location$Address", ClassHelper.AllFlag);
            Assert.IsTrue(addr.HasAttribute<AllowNullAttribute>(false));
            var len = addr.GetAttribute<LengthAttribute>(false);
            Assert.IsNotNull(len);
            Assert.AreEqual(2, len.Min);
            Assert.AreEqual(50, len.Max);

            var num = type.GetProperty("$Location$Number", ClassHelper.AllFlag);
            var nn = num.GetAttribute<DbColumnAttribute>(false);
            Assert.AreEqual("MyNumber", nn.Name);
            Assert.AreEqual(2, num.GetCustomAttributes(false).Length);

            var ph = type.GetProperty("$Location$Phone", ClassHelper.AllFlag);
            var pn = ph.GetAttribute<DbColumnAttribute>(false);
            Assert.AreEqual("LocationPhone", pn.Name);
            Assert.AreEqual(2, num.GetCustomAttributes(false).Length);
        }

        [Test]
        public void Test4() {
            var type = typeof(CoAddr);

            var city1 = type.GetProperty("$MyAddress$City", ClassHelper.AllFlag);
            Assert.IsNotNull(city1);
            var pn1 = city1.GetAttribute<DbColumnAttribute>(false);
            Assert.AreEqual("MyAddressCity", pn1.Name);

            var city2 = type.GetProperty("$YourAddress$City", ClassHelper.AllFlag);
            Assert.IsNotNull(city2);
            var pn2 = city2.GetAttribute<DbColumnAttribute>(false);
            Assert.AreEqual("YourAddressCity", pn2.Name);
        }
    }
}