using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using Leafing.Core.Text;

namespace Leafing.UnitTest.util
{
    [TestFixture]
    public class XDictionaryTest
    {
        [Test]
        public void Test1()
        {
            var dictionary = new XDictionary<string, string> {{"test", "ok"}, {"run", "fine"}};
            string act = XmlSerializer<XDictionary<string, string>>.Xml.Serialize(dictionary);

			var root = XElement.Parse (act);
			Assert.AreEqual ("dictionary", root.Name.LocalName);
			int n = 0;
			var values = new string[] {"test", "ok", "run", "fine"};
			foreach (var item in root.Elements("item")) {
				Assert.AreEqual (values[n], item.Attribute ("key").Value);
				Assert.AreEqual (values[n+1], item.Attribute ("value").Value);
				n += 2;
			}
			Assert.AreEqual (4, n);
        }

        [Test]
        public void Test2()
        {
            const string src = @"<?xml version=""1.0""?>
<dictionary>
  <item key=""test"" value=""ok"" />
  <item key=""run"" value=""fine"" />
</dictionary>";

            XDictionary<string, string> dictionary = XmlSerializer<XDictionary<string, string>>.Xml.Deserialize(src);
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual("ok", dictionary["test"]);
            Assert.AreEqual("fine", dictionary["run"]);
        }
    }
}
