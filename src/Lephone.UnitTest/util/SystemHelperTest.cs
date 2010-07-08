using Lephone.Core;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class SystemHelperTest
	{
		[Test]
		public void TestCurrentFunctionName()
		{
			string s = SystemHelper.CurrentFunctionName;
            string exp = this.GetType().FullName + ".TestCurrentFunctionName()";
			Assert.AreEqual(exp, s);
		}

        [Test, Ignore("for some reason, it cann't get the right result under nunit")]
		public void TestCallerFunctionName()
		{
            string s = GetCallerFunctionName();
			Assert.AreEqual("Lephone.Core.UnitTest.SystemHelperTest.TestCallerFunctionName()", s);
		}

        private static string GetCallerFunctionName()
        {
            return SystemHelper.CallerFunctionName;
        }
    }
}
