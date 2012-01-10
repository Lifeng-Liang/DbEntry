using Leafing.Web;
using NUnit.Framework;

namespace Leafing.UnitTest.Web
{
    [TestFixture]
    public class XmlBuilderTest
    {
        [Test]
        public void Test1()
        {
            XmlBuilder b = XmlBuilder.New(null, null).tag("User").attr("Age", 13).attr("Class", 7).text("tom").end;
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<User Age=\"13\" Class=\"7\">tom</User>", b.ToString());
        }

        [Test]
        public void Test2()
        {
            XmlBuilder b = XmlBuilder.New().tag("Book").end;
            Assert.AreEqual("<Book />", b.ToString());
        }

        [Test]
        public void Test3()
        {
            XmlBuilder b = XmlBuilder.New().tag("Books").enter().tab.tag("Book").text("DbEntry.Net Development").end.end;
            Assert.AreEqual("<Books>\r\n\t<Book>DbEntry.Net Development</Book></Books>", b.ToString());
        }
    }
}
