using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class SerializeTest
    {
        [Serializable]
        public class User : DbObjectModel<User>
        {
            public string Name { get; set; }
        }

        [Test]
        public void Test1()
        {
            var u = new User {Id = 3, Name = "tom"};

            IFormatter formatter = new BinaryFormatter();
            using(var stream = new MemoryStream())
            {
                formatter.Serialize(stream, u);
                stream.Position = 0;
                var u1 = (User)formatter.Deserialize(stream);
                Assert.AreEqual(3, u1.Id);
                Assert.AreEqual("tom", u1.Name);
            }
        }
    }
}
