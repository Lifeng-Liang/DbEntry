using System;
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
            const string s = "Select * from User where Age > ? And Age < ?";
            const string exp = "Select * from User where Age > @p0 And Age < @p1";
            var reg = new Regex("\\?");
            string act = reg.Replace(s, "@p0", 1);
            act = reg.Replace(act, "@p1", 1);
            Assert.AreEqual(exp, act);
        }

        [Test]
        public void TestForReplace2()
        {
            const string s = "Select * from User where Id = ? Name Like '%?%' Age > ? And Age < ? ";
            const string exp = "Select * from User where Id = @p0 Name Like '%?%' Age > @p1 And Age < @p2 ";
            var reg = new Regex("'.*'|\\?");
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

        [Test]
        public void TestConvert()
        {
            var n = (int)Convert.ChangeType("18", typeof (int));
            Assert.AreEqual(18, n);

            var b = (bool) Convert.ChangeType("true", typeof (bool));
            Assert.AreEqual(true, b);
        }
    }
}
