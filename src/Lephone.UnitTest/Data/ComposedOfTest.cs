using Lephone.Core;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public interface ILocation
    {
        string Phone { get; set; }

        [AllowNull, Length(2, 50)]
        string Address { get; set; }

        [DbColumn("MyNumber")]
        int Number { get; set; }

        int? Wow { get; set; }
    }

    [TestFixture]
    public class ComposedOfTest : DataTestBase
    {
        public class CoUser : DbObjectModel<CoUser>
        {
            public string Name { get; set; }

            [ComposedOf]
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

        [Test]
        public void Test2()
        {
            var user = new CoUser { Name = "tom", Location = { Phone = "123456", Address = "The east of queen street" } };
            user.Save();

            user = CoUser.FindById(user.Id);
            user.Location.Address = "test";
            user.Save();

            user = CoUser.FindById(user.Id);
            Assert.AreEqual("test", user.Location.Address);
        }

        [Test]
        public void Test3()
        {
            var type = typeof(CoUser);
            var addr = type.GetProperty("$Location$Address", ClassHelper.AllFlag);
            Assert.IsTrue(ClassHelper.HasAttribute<AllowNullAttribute>(addr, false));
            var len = ClassHelper.GetAttribute<LengthAttribute>(addr, false);
            Assert.IsNotNull(len);
            Assert.AreEqual(2, len.Min);
            Assert.AreEqual(50, len.Max);

            var num = type.GetProperty("$Location$Number", ClassHelper.AllFlag);
            var nn = ClassHelper.GetAttribute<DbColumnAttribute>(num, false);
            Assert.AreEqual("MyNumber", nn.Name);
            Assert.AreEqual(1, num.GetCustomAttributes(false).Length);

            var ph = type.GetProperty("$Location$Phone", ClassHelper.AllFlag);
            var pn = ClassHelper.GetAttribute<DbColumnAttribute>(ph, false);
            Assert.AreEqual("LocationPhone", pn.Name);
            Assert.AreEqual(1, num.GetCustomAttributes(false).Length);
        }
    }
}
