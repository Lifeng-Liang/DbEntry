using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
	internal class TesterHelper
	{
		private static string ReplaceSqlString(string s)
		{
			string ret = s.Replace("  ", " ");
			ret = ret.Trim();
			return ret;
		}

		// TODO: It should be using Regular to check
		public static void AssertSqlSentenceEqual(string Exp, string Dst)
		{
			Exp = ReplaceSqlString(Exp);
			Dst = ReplaceSqlString(Dst);
			Assert.AreEqual(Exp, Dst);
		}
	}
}
