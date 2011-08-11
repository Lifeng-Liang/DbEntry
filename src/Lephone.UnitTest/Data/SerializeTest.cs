using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Lephone.Core.Text;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class SerializeTest : DataTestBase
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

        [Test]
        public void Test2()
        {
            var list = vPeople.FindAll(p => p.Id);
            var xml = XmlSerializer<List<vPeople>>.Xml.Serialize(list);
            Assert.AreEqual(@"<?xml version=""1.0""?>
<List_x0060_1 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <vPeople>
    <Id>1</Id>
    <Name>Tom</Name>
  </vPeople>
  <vPeople>
    <Id>2</Id>
    <Name>Jerry</Name>
  </vPeople>
  <vPeople>
    <Id>3</Id>
    <Name>Mike</Name>
  </vPeople>
</List_x0060_1>", xml);
        }
    }
}
