using Leafing.Core.Ioc;
using Leafing.UnitTest.Mocks;
using Leafing.Web.Mvc;
using NUnit.Framework;

namespace Leafing.UnitTest.Mvc
{
    public class TestController : ControllerBase
    {
        public IView Add(int d1, int d2)
        {
            var result = d1 + d2;
            var v = new TextView();
            v.Append(result.ToString());
            return v;
        }

        public IView Get(string s1, string s2, string s3, int i4, long l5)
        {
            var v = new TextView();
            v.Append(s1).Append(",");
            v.Append(s2).Append(",");
            v.Append(s3).Append(",");
            v.Append(i4.ToString()).Append(",");
            v.Append(l5.ToString());
            return v;
        }

        public IView GetAll(string s1, string s2, string s3, string s4, string s5)
        {
            var v = new TextView();
            v.Append(s1).Append("|");
            v.Append(s2).Append("|");
            v.Append(s3).Append("|");
            v.Append(s4).Append("|");
            v.Append(s5);
            return v;
        }
    }

    [TestFixture]
    public class TextViewTest
    {
        [Test]
        public void TestAdd()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/test/add/13/25";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("38", MockHttpContextHandler.LastWriteMessage);
        }

        [Test]
        public void TestGet()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/test/get/test/ok/3rd/18/30";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("test,ok,3rd,18,30", MockHttpContextHandler.LastWriteMessage);
        }

        [Test]
        public void TestGetAll()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/test/getall/test/ok/3rd/18/30";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("test|ok|3rd|18|30", MockHttpContextHandler.LastWriteMessage);
        }
    }
}
