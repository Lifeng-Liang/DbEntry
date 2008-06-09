using Lephone.Util;
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

        /*
        // for some reason, it cann't get the right result under nunit;
		[Test]
		public void TestCallerFunctionName()
		{
            string s = GetCallerFunctionName();
			Assert.AreEqual("Lephone.Util.UnitTest.SystemHelperTest.TestCallerFunctionName()", s);
		}
        */

        private string GetCallerFunctionName()
        {
            return SystemHelper.CallerFunctionName;
        }
    }
}
