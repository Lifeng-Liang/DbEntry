using System.Collections.Generic;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
{
    [TestFixture]
    public class HashTast
    {
        [Test]
        public void Test1()
        {
            var u = new User {Name = "tom", Type = UserType.Manager, Valid = true};
            var h1 = u.GetHashCode();
            var list = new List<User> {u};
            u.Type = UserType.Administrator;
            var u1 = list[0];
            Assert.AreEqual(UserType.Administrator, u1.Type);
            Assert.AreEqual(h1, u1.GetHashCode());
        }
    }
}
