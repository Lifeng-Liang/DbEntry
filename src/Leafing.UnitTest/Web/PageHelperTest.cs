using System;
using NUnit.Framework;
using Leafing.Web.Common;

namespace Leafing.UnitTest.Web
{
	[TestFixture]
	public class PageHelperTest
	{
		[Test]
		public void TestGetCssBase()
		{
			var s = PageHelper.GetCssBase(null);
			Assert.AreEqual("", s);

			s = PageHelper.GetCssBase("");
			Assert.AreEqual("", s);

			s = PageHelper.GetCssBase("test");
			Assert.AreEqual("test ", s);

			s = PageHelper.GetCssBase("test ok");
			Assert.AreEqual("test ok ", s);
		}

		[Test]
		public void TestGetOriginCss()
		{
			var s = PageHelper.GetOriginCss(null, "warn");
			Assert.AreEqual("", s);

			s = PageHelper.GetOriginCss("", "warn");
			Assert.AreEqual("", s);

			s = PageHelper.GetOriginCss("warn", "warn");
			Assert.AreEqual("", s);

			s = PageHelper.GetOriginCss("test", "warn");
			Assert.AreEqual("test", s);

			s = PageHelper.GetOriginCss("testwarn", "warn");
			Assert.AreEqual("testwarn", s);

			s = PageHelper.GetOriginCss("test warn", "warn");
			Assert.AreEqual("test", s);
		}
	}
}

