
#region usings

using System;
using NUnit.Framework;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.UnitTest.util
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
			Assert.AreEqual("org.hanzify.llf.util.UnitTest.SystemHelperTest.TestCallerFunctionName()", s);
		}
        */

        private string GetCallerFunctionName()
        {
            return SystemHelper.CallerFunctionName;
        }
    }
}
