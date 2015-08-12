using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Leafing.Data.Definition;
using Leafing.Core.Text;
using Leafing.Extra;
using NUnit.Framework;
using System.Xml.Linq;

namespace Leafing.UnitTest.util
{
    [TestFixture]
    public class XmlSerializerTest
    {
        [XmlRoot("List")]
        public class MyList2 : List<MyItem> { }

        [XmlType("List")]
        public class MyList : List<MyItem> { }

        [XmlType("Item")]
        public class MyItem
        {
            public string Name;

            public MyItem() { }

            public MyItem(string name)
            {
                this.Name = name;
            }
        }

        [Serializable]
        public class Sitex : XmlSerializableDbObjectModel<Sitex>
        {
            public string Url { get; set; }
        }

        [Test]
        public void Test1()
        {
            var l = new MyList {new MyItem("tom")};
			string act = XmlSerializer<MyList>.Xml.Serialize(l);
			var root = XElement.Parse(act);
			Assert.AreEqual ("List", root.Name.LocalName);
			Assert.AreEqual ("tom", root.Element("Item").Element("Name").Value);
        }

        [Test]
        public void Test2()
        {
            var l = new MyList2 {new MyItem("tom")};
            string act = XmlSerializer<MyList2>.Xml.Serialize(l);
			var root = XElement.Parse (act);
			Assert.AreEqual ("List", root.Name.LocalName);
			Assert.AreEqual ("tom", root.Element("Item").Element("Name").Value);
        }

        [Test]
        public void Test3()
        {
            var s = new Sitex {Url = "ddd"};
            string c2 = XmlSerializer<Sitex>.Xml.Serialize(s);

			var root = XElement.Parse (c2);
			Assert.AreEqual ("Sitex", root.Name.LocalName);
			Assert.AreEqual ("0", root.Element ("Id").Value);
			Assert.AreEqual ("ddd", root.Element ("Url").Value);
//            Assert.AreEqual(@"<?xml version=""1.0""?>
//<Sitex>
//  <Id>0</Id>
//  <Url>ddd</Url>
//</Sitex>", c2);

            var f = XmlSerializer<Sitex>.Xml.Deserialize(c2);
            Assert.AreEqual("ddd", f.Url);
        }

        [Test]
        public void TestSitexSchema()
        {
            var s = new Sitex {Url = "ddd"};
            string c2 = XmlSerializer<Sitex>.Xml.Serialize(s);
            var schema = ((IXmlSerializable)s).GetSchema();
            var set = new XmlSchemaSet();
            set.Add(schema);
            var reader = XmlReader.Create(
                new MemoryStream(Encoding.UTF8.GetBytes(c2)),
                new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = set });
            while (reader.Read())
            {
            }
        }

        [Test]
        public void TestSitexSchema2()
        {
            const string ss = @"<?xml version=""1.0""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">

<xs:element name=""Sitex"">
  <xs:complexType>
    <xs:sequence>
      <xs:element name=""Id"" type=""xs:long""/>
      <xs:element name=""Url"" type=""xs:string""/>
    </xs:sequence>
  </xs:complexType>
</xs:element>

</xs:schema>";
            var schema = XmlSchema.Read(new MemoryStream(Encoding.UTF8.GetBytes(ss)), null);

            var s = new Sitex { Url = "ddd" };
            string c2 = XmlSerializer<Sitex>.Xml.Serialize(s);
            var set = new XmlSchemaSet();
            set.Add(schema);
            var reader = XmlReader.Create(
                new MemoryStream(Encoding.UTF8.GetBytes(c2)),
                new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = set });
            while (reader.Read())
            {
            }
        }
    }
}
