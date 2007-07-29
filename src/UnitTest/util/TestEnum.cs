
#region usings

using System;
using org.hanzify.llf.util.Text;

#endregion

namespace org.hanzify.llf.UnitTest.util
{
	internal enum TestEnum
	{
		[ShowString("MyTest")] Test1,
		[ShowString("Test")] Test2,
		[ShowString("Rand")] Test3,
		[ShowString("Sp")] Test4,
		Test5
	}
}
