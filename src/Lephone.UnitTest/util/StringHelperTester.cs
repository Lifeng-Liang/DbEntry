using System;
using Lephone.Core;
using Lephone.Core.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class StringHelperTester
	{
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test()
        {
            const string s = "";
            StringHelper.GetStringLeft(s);
        }

	    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test1()
	    {
	        const string s = "abc";
	        StringHelper.GetStringLeft(s, 3);
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
			const string s = "_abc";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test5()
		{
			const string s = "_ab123c";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test6()
		{
			const string s = "ab123c";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test7()
		{
			const string s = "__abc12";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test8()
		{
			const string s = "a";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test9()
		{
			const string s = "_";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test10()
		{
			const string s = "1abc12";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test11()
		{
			const string s = "1";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test12()
		{
			const string s = "%";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test13()
		{
			const string s = "ab%";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test14()
		{
			const string s = "";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test15()
		{
			const string s = "ab\ncd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test16()
		{
			const string s = "ab\tcd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test17()
		{
			const string s = "ab cd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(false, b);
		}

		[Test]
		public void Test18()
		{
			const string s = " abcd ";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test19()
		{
			const string s = "\t abcd";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

		[Test]
		public void Test20()
		{
			const string s = "abcd\t";
			bool b = StringHelper.IsIndentityName(s);
			Assert.AreEqual(true, b);
		}

        [Test]
        public void Test21()
        {
            const string s = "aaaa";
            string[] ss = StringHelper.Split(s, ':', 2);
            Assert.AreEqual(2, ss.Length);
            Assert.AreEqual("aaaa", ss[0]);
            Assert.AreEqual("", ss[1]);
        }

        [Test]
        public void Test22()
        {
            const string s = "aa:a:a";
            string[] ss = StringHelper.Split(s, ':', 2);
            Assert.AreEqual(2, ss.Length);
            Assert.AreEqual("aa", ss[0]);
            Assert.AreEqual("a:a", ss[1]);
        }

        [Test]
        public void TestByteArray()
        {
            var b1 = new byte[] { 1, 2, 3, 4 };
            var b2 = new byte[] { 1, 2, 3, 4 };
            Assert.IsFalse(b1 == b2);
        }

        [Test]
        public void TestAreEqual()
        {
            var b1 = new byte[] { 1, 2, 3, 4 };
            var b2 = new byte[] { 1, 2, 3, 4 };
            var ret = CommonHelper.AreEqual(b1, b2);
            Assert.IsTrue(ret);
            var b3 = new byte[] { 1, 2, 3, 5 };
            Assert.IsFalse(CommonHelper.AreEqual(b1, b3));
        }

        [Test]
        public void TestIsSpName()
        {
            Assert.IsTrue(StringHelper.IsSpName("abc"));
            Assert.IsTrue(StringHelper.IsSpName("_abc_"));
            Assert.IsTrue(StringHelper.IsSpName("_abc_._zzz_"));
            Assert.IsTrue(StringHelper.IsSpName("abc.zzz"));
            Assert.IsTrue(StringHelper.IsSpName(" abc "));
            Assert.IsTrue(StringHelper.IsSpName(" _abc_ "));
            Assert.IsTrue(StringHelper.IsSpName(" abc.abc "));
            Assert.IsTrue(StringHelper.IsSpName(" a11.z99 "));
            Assert.IsTrue(StringHelper.IsSpName(" Z09.a48 "));
            Assert.IsTrue(StringHelper.IsSpName(" _Z09_._a48_ "));

            Assert.IsFalse(StringHelper.IsSpName(" a11.111 "));
            Assert.IsFalse(StringHelper.IsSpName(" 11a.abc "));
            Assert.IsFalse(StringHelper.IsSpName(" _1abc._abc "));
            Assert.IsFalse(StringHelper.IsSpName(" _abc.#abc "));
            Assert.IsFalse(StringHelper.IsSpName(" _abc. abc "));
            Assert.IsFalse(StringHelper.IsSpName(" abc.abc.abc "));
        }

        [Test]
        public void TestProcessSymbol()
        {
            var result = StringHelper.ProcessSymbol("[[abc)", "[[", ")", text => text);
            Assert.AreEqual("abc", result);

            result = StringHelper.ProcessSymbol("1111[[[[abc)2222", "[[[[", ")", text => text);
            Assert.AreEqual("1111abc2222", result);

            result = StringHelper.ProcessSymbol("1111(abc]]]]2222", "(", "]]]]", text => text);
            Assert.AreEqual("1111abc2222", result);
        }

        [Test]
        public void TestSplitByCase()
        {
            var list = StringHelper.SplitByCase("nameAndAge");
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("name", list[0]);
            Assert.AreEqual("And", list[1]);
            Assert.AreEqual("Age", list[2]);
        }

        [Test]
        public void TestSplitByCase2()
        {
            var list = StringHelper.SplitByCase("NameAndAge");
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Name", list[0]);
            Assert.AreEqual("And", list[1]);
            Assert.AreEqual("Age", list[2]);
        }

        [Test]
        public void TestSplitByCase3()
        {
            var list = StringHelper.SplitByCase("name");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("name", list[0]);
        }

        [Test]
        public void TestSplitByCase4()
        {
            var list = StringHelper.SplitByCase("Name");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Name", list[0]);
        }
    }
}
