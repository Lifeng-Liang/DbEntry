
#region usings

using System;
using Lephone.Util.Text;

#endregion

namespace Lephone.UnitTest.util
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
