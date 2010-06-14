using System.Collections.Generic;
using NUnit.Framework;
using Lephone.Core.Text;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class XDictionaryTest
    {
        [Test]
        public void Test1()
        {
            var dictionary = new XDictionary<string, string> {{"test", "ok"}, {"run", "fine"}};
            string act = XmlSerializer<XDictionary<string, string>>.Xml.Serialize(dictionary);
            const string exp = @"<?xml version=""1.0""?>
<dictionary>
  <item key=""test"" value=""ok"" />
  <item key=""run"" value=""fine"" />
</dictionary>";
            Assert.AreEqual(exp, act);
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
