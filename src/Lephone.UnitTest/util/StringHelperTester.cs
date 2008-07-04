using System;
using Lephone.Util.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class StringHelperTester
	{
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test()
		{
			string s = "";
			s = StringHelper.GetStringLeft(s);
		}

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test1()
		{
			string s = "abc";
			s = StringHelper.GetStringLeft(s, 3);
		}

		[Test]
		public void Test2()
		{
			string s = "abc";
			s = StringHelper.GetStringLeft(s);
			Assert.AreEqual(s, "ab");
		}

		[Test]
		public void Test3()
		{
			string s = "a,b,c,";
			s = StringHelper.GetStringLeft(s);
			Assert.AreEqual(s, "a,b,c");
		}

		[Test]
		public void Test4()
		{
			string s = "_abc";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test5()
		{
			string s = "_ab123c";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test6()
		{
			string s = "ab123c";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test7()
		{
			string s = "__abc12";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test8()
		{
			string s = "a";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test9()
		{
			string s = "_";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test10()
		{
			string s = "1abc12";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test11()
		{
			string s = "1";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test12()
		{
			string s = "%";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test13()
		{
			string s = "ab%";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test14()
		{
			string s = "";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test15()
		{
			string s = "ab\ncd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test16()
		{
			string s = "ab\tcd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test17()
		{
			string s = "ab cd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test18()
		{
			string s = " abcd ";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test19()
		{
			string s = "\t abcd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test20()
		{
			string s = "abcd\t";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

        [Test]
        public void Test21()
        {
            string s = "aaaa";
            string[] ss = StringHelper.Split(s, ':', 2);
            Assert.AreEqual(2, ss.Length);
            Assert.AreEqual("aaaa", ss[0]);
            Assert.AreEqual("", ss[1]);
        }

        [Test]
        public void Test22()
        {
            string s = "aa:a:a";
            string[] ss = StringHelper.Split(s, ':', 2);
            Assert.AreEqual(2, ss.Length);
            Assert.AreEqual("aa", ss[0]);
            Assert.AreEqual("a:a", ss[1]);
        }
    }
}
