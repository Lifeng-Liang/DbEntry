
#region usings

using System;
using NUnit.Framework;
using Lephone.Util.Text;

#endregion

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class EnumTester
	{
		[Test]
		public void Test()
		{
			string s = StringHelper.EnumToString(TestEnum.Test1);
			Assert.AreEqual(s, "MyTest");
			s = StringHelper.EnumToString(TestEnum.Test2);
			Assert.AreEqual(s, "Test");
			s = StringHelper.EnumToString(TestEnum.Test3);
			Assert.AreEqual(s, "Rand");
			s = StringHelper.EnumToString(TestEnum.Test4);
			Assert.AreEqual(s, "Sp");
			s = StringHelper.EnumToString(TestEnum.Test5);
			Assert.AreEqual(s, "Test5");
		}
	}
}
