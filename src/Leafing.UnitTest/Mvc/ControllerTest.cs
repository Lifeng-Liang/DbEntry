using Leafing.Core.Ioc;
using Leafing.UnitTest.Mocks;
using Leafing.Web;
using Leafing.Web.Mvc;
using NUnit.Framework;

namespace Leafing.UnitTest.Mvc
{
    public class MyView : IView
    {
        public static ControllerBase Controller;

        public MyView(ControllerBase controller)
        {
            Controller = controller;
        }

        public void Render()
        {
        }
    }

    public class MyController : ControllerBase
    {
        public IView Add(int d1, int d2)
        {
            this["Result"] = d1 + d2;
            return new MyView(this);
        }

        public IView Get(string s1, int i2, long l3)
        {
            this["s1"] = s1;
            this["i2"] = i2;
            this["l3"] = l3;
            return new MyView(this);
        }

        public IView GetAll(string s1, string s2, string s3)
        {
            this["s1"] = s1;
            this["s2"] = s2;
            this["s3"] = s3;
            return new MyView(this);
        }

        public IView Try(string s1, string s2)
        {
            this["s1"] = s1;
            this["s2"] = s2;
            return new MyView(this);
        }

        protected override void OnException(System.Exception ex)
        {
            throw ex;
        }
    }

    [TestFixture]
    public class ControllerTest
    {
        [Test]
        public void TestAdd()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/add/13/25";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual(38, MyView.Controller["Result"]);
        }

        [Test]
        public void TestGet()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/get/ok/18/30";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("ok", MyView.Controller["s1"]);
            Assert.AreEqual(18, MyView.Controller["i2"]);
            Assert.AreEqual(30, MyView.Controller["l3"]);
        }

        [Test]
        public void TestGetAll()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/getall/3rd/18/30";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("3rd", MyView.Controller["s1"]);
            Assert.AreEqual("18", MyView.Controller["s2"]);
            Assert.AreEqual("30", MyView.Controller["s3"]);
        }

        [Test]
        public void TestTry()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/try/first";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
            Assert.AreEqual("first", MyView.Controller["s1"]);
            Assert.AreEqual(null, MyView.Controller["s2"]);
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "The count of paremeters for Action [My.Try] is wrong!")]
        public void TestTryException()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/try/first/1/1/2";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "Controller [nothing] not found!")]
        public void TestNoController()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/nothing/try/first/1/1/2";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "Action [My.nothing] not found!")]
        public void TestNoAction()
        {
            MockHttpContextHandler.MockAppRelativeCurrentExecutionFilePath = "~/my/nothing/first/1/1/2";
            var p = SimpleContainer.Get<MvcProcessor>();
            p.Process();
        }
    }
}
