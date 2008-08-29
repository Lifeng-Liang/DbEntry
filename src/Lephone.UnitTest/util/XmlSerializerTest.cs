using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Lephone.Util.Text;
using Lephone.Linq;
using NUnit.Framework;

namespace Lephone.UnitTest.util
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

            public MyItem(string Name)
            {
                this.Name = Name;
            }
        }

        [Serializable]
        public abstract class Sitex : LinqObjectModel<Sitex>
        {
            public abstract string Url { get; set; }
        }

        [Test]
        public void Test1()
        {
            MyList l = new MyList();
            l.Add(new MyItem("tom"));
            string act = XmlSerializer<MyList>.Xml.Serialize(l);
            Assert.AreEqual("<?xml version=\"1.0\"?>\r\n<List xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Item>\r\n    <Name>tom</Name>\r\n  </Item>\r\n</List>", act);
        }

        [Test]
        public void Test2()
        {
            MyList2 l = new MyList2();
            l.Add(new MyItem("tom"));
            string act = XmlSerializer<MyList2>.Xml.Serialize(l);
            Assert.AreEqual("<?xml version=\"1.0\"?>\r\n<List xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Item>\r\n    <Name>tom</Name>\r\n  </Item>\r\n</List>", act);
        }

        [Test, Ignore("waiting for a full supported ReadXml function, Maybe include GetSchema")]
        public void Test3()
        {
            Sitex s = Sitex.New();
            s.Url = "ddd";
            string c2 = XmlSerializer<Sitex>.Xml.Serialize(s);
            Assert.AreEqual(@"<?xml version=""1.0""?>
<Sitex>
  <Id>0</Id>
  <Url>ddd</Url>
</Sitex>", c2);

            var f = XmlSerializer<Sitex>.Xml.Deserialize(c2);
            Assert.AreEqual("ddd", f.Url);
        }
    }
}
