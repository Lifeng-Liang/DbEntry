﻿using System.Text;
using Lephone.Core;
using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class UrlBuilderTest
    {
        [Test]
        public void TestNormal()
        {
            UrlBuilder ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "DbEntry");
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=DbEntry", dst);
        }

        [Test]
        public void TestUnicode()
        {
            UrlBuilder ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "vb", Encoding.Unicode);
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=v%00b%00", dst);
        }

        [Test]
        public void TestUnicodeAsDefault()
        {
            UrlBuilder ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp", Encoding.Unicode);
            ub.Add("Where", "title");
            ub.Add("keyword", "vb");
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=t%00i%00t%00l%00e%00&keyword=v%00b%00", dst);
        }

        [Test]
        public void TestChineseGBK()
        {
            UrlBuilder ub = new UrlBuilder("http://llf.hanzify.org/llf/search.asp");
            ub.Add("Where", "title");
            ub.Add("keyword", "中文", EncodingEx.GBK);
            string dst = ub.ToString();
            Assert.AreEqual("http://llf.hanzify.org/llf/search.asp?Where=title&keyword=%d6%d0%ce%c4", dst);
        }
    }
}
