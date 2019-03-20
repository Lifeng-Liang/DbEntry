using Leafing.Core;
using NUnit.Framework;

namespace Leafing.UnitTest.util {
    [TestFixture]
    public class SystemHelperTest {
        [Test, Ignore("for some reason, it cann't get the right result under nunit in release mode")]
        public void TestCurrentFunctionName() {
            string s = SystemHelper.CurrentFunctionName;
            string exp = this.GetType().FullName + ".TestCurrentFunctionName()";
            Assert.AreEqual(exp, s);
        }

        [Test, Ignore("for some reason, it cann't get the right result under nunit")]
        public void TestCallerFunctionName() {
            string s = GetCallerFunctionName();
            Assert.AreEqual("Leafing.Core.UnitTest.SystemHelperTest.TestCallerFunctionName()", s);
        }

        private static string GetCallerFunctionName() {
            return SystemHelper.CallerFunctionName;
        }
    }
}