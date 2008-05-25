using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
{
    [TestFixture]
    public class TextTest
    {
        [Test]
        public void TestForReplace()
        {
            string s = "Select * from User where Age > ? And Age < ?";
            string exp = "Select * from User where Age > @p0 And Age < @p1";
            Regex reg = new Regex("\\?");
            string act = reg.Replace(s, "@p0", 1);
            act = reg.Replace(act, "@p1", 1);
            Assert.AreEqual(exp, act);
        }

        [Test]
        public void TestForReplace2()
        {
            string s = "Select * from User where Id = ? Name Like '%?%' Age > ? And Age < ? ";
            string exp = "Select * from User where Id = @p0 Name Like '%?%' Age > @p1 And Age < @p2 ";
            Regex reg = new Regex("'.*'|\\?");
            int start = 0, n = 0;
            string act = "";
            foreach (Match m in reg.Matches(s))
            {
                if (m.Length == 1)
                {
                    act += s.Substring(start, m.Index - start);
                    act += "@p" + n++;
                    start = m.Index + 1;
                }
            }
            if (start < s.Length)
            {
                act += s.Substring(start);
            }
            Assert.AreEqual(exp, act);
        }
    }
}
