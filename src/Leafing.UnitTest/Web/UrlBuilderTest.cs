using System.Text;
using Leafing.Core;
using Leafing.Web;
using NUnit.Framework;

namespace Leafing.UnitTest.Web
{
    [TestFixture]
    public class UrlBuilderTest
    {
        [Test]
        public void TestNormal()
        {
            var ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "DbEntry");
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=DbEntry", dst);
        }

        [Test]
        public void TestUnicode()
        {
            var ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "vb", Encoding.Unicode);
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=v%00b%00", dst);
        }

        [Test]
        public void TestUnicodeAsDefault()
        {
            var ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp", Encoding.Unicode);
            ub.Add("Where", "title");
            ub.Add("keyword", "vb");
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=t%00i%00t%00l%00e%00&keyword=v%00b%00", dst);
        }

        [Test]
        public void TestChineseGbk()
        {
            var ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "中文", Util.GetGbkEncoding());
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=%d6%d0%ce%c4", dst);
        }
    }
}
