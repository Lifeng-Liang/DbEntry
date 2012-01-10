using Leafing.Web;
using Leafing.Web.Mvc;
using NUnit.Framework;

namespace Leafing.UnitTest.Mvc
{
    public class YourController : ControllerBase
    {
        public void Show(int n)
        {
            this["Test"] = n;
        }

        public void Pick(string name)
        {
        }

        public void List(int? index)
        {
        }

        public void Add(int? a, int? b)
        {
        }

        public void New()
        {
        }

        public string Next()
        {
            return null;
        }
    }

    public class ThatController : ControllerBase
    {
        public void Show()
        {
        }
    }

    [TestFixture]
    public class ControllerBaseTest
    {
        [Test]
        public void TestUrlTo()
        {
            var ctrl = new YourController();
            var r1 = ctrl.UrlTo<ThatController>(p => p.Show());
            Assert.AreEqual("http://dbentry.codeplex.com/that/show", r1.ToString());
            var r2 = ctrl.UrlTo<YourController>(p => p.Show(2));
            Assert.AreEqual("http://dbentry.codeplex.com/your/show/2", r2.ToString());
        }

        [Test]
        public void TestUrlToNull()
        {
            var ctrl = new YourController();
            var r1 = ctrl.UrlTo<YourController>(p => p.List(null));
            Assert.AreEqual("http://dbentry.codeplex.com/your/list", r1.ToString());
        }

        [Test]
        public void TestAddFirst()
        {
            var ctrl = new YourController();
            var r1 = ctrl.UrlTo<YourController>(p => p.Add(12, null));
            Assert.AreEqual("http://dbentry.codeplex.com/your/add/12", r1.ToString());
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "Can not pass NULL in the middle of the parameters")]
        public void TestAddSecend()
        {
            var ctrl = new YourController();
            ctrl.UrlTo<YourController>(p => p.Add(null, 13));
        }

        [Test]
        public void TestAdd()
        {
            var ctrl = new YourController();
            var r1 = ctrl.UrlTo<YourController>(p => p.Add(5 * 8, 13));
            Assert.AreEqual("http://dbentry.codeplex.com/your/add/40/13", r1.ToString());
        }

        [Test]
        public void TestToController()
        {
            var ctrl = new YourController();
            var r1 = ctrl.UrlTo<YourController>();
            Assert.AreEqual("http://dbentry.codeplex.com/your", r1.ToString());
        }

        public void Test()
        {
            var ctrl = new YourController();
            ctrl.Show(2);
            dynamic row = ctrl.GetValues();
            Assert.AreEqual(2, row.Test);
        }
    }
}
