using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Leafing.Core.Text;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;
using System.Xml.Linq;

namespace Leafing.UnitTest.Data
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
			var list = vPeople.Where (Condition.Empty).OrderBy (p => p.Id).Select ();
			var xml = XmlSerializer<List<vPeople>>.Xml.Serialize (list);

			var root = XElement.Parse (xml);
			Assert.AreEqual ("List_x0060_1", root.Name.LocalName);
			var nodes = new List<XElement> ();
			nodes.AddRange (root.Elements());
			Assert.AreEqual (3, nodes.Count);
			Assert.AreEqual ("1", nodes [0].Element ("Id").Value);
			Assert.AreEqual ("Tom", nodes [0].Element ("Name").Value);
			Assert.AreEqual ("2", nodes [1].Element ("Id").Value);
			Assert.AreEqual ("Jerry", nodes [1].Element ("Name").Value);
			Assert.AreEqual ("3", nodes [2].Element ("Id").Value);
			Assert.AreEqual ("Mike", nodes [2].Element ("Name").Value);
		}
    }
}
