using System;
using NUnit.Framework;
using Leafing.Web.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leafing.Data.Definition;
using Leafing.Data;
using Leafing.Web;
using Leafing.Core.Text;

namespace Leafing.UnitTest.Web
{
	public class User : DbObjectModel<User>
	{
		[Length(6,8)]
		public string Name;
		[Index(UNIQUE = true), ShowString("TestField")]
		public int Age;
	}
	
	[TestFixture]
	public class PageHelperTest : DataTestBase
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

		public class MyPage : Page
		{
			private TextBox User_Name;
			private TextBox User_Age;

			public MyPage(TextBox name, TextBox age)
			{
				User_Name = name;
				User_Age = age;
			}
			protected override void OnInit(EventArgs e)
			{
				base.OnInit(e);
				Controls.Add(User_Name);
				Controls.Add(User_Age);
			}
		}

		[Test]
		public void TestPage()
		{
			DbEntry.Create(typeof(User));
			var name = new TextBox();
			name.ID = "user_name";
			name.Text = "tom";
			name.CssClass = "test";
			var age = new TextBox();
			age.ID = "user_age";
			age.Text = "18";
			age.CssClass = "test";
			var label = new Label();
			label.CssClass = "msg";
			var page = new MyPage(name, age);
			page.Controls.Add(label);
			var msg = new NoticeLabelAdapter(label, "notice", "warning");
			var ctx = ModelContext.GetInstance(typeof(User));
			var vh = new ValidateHandler();
			var obj = ctx.GetObject<User>(page, "parse error");
			ctx.ValidateSave(page, vh, obj, msg, "hello", "inWarning", () => obj.Save());
			Assert.AreEqual("test inWarning", name.CssClass);
			Assert.AreEqual("test", age.CssClass);
			Assert.AreEqual("msg warning", label.CssClass);
			Assert.AreEqual("<ul>\r\n<li>Invalid Field Name The length should be 6 to 8 but was 3.</li>\r\n</ul>\r\n", label.Text);

			name.Text = "tom456789";
			vh = new ValidateHandler();
			obj = ctx.GetObject<User>(page, "parse error");
			msg = new NoticeLabelAdapter(label, "notice", "warning");
			ctx.ValidateSave(page, vh, obj, msg, "hello", "inWarning", () => obj.Save());
			Assert.AreEqual("test inWarning", name.CssClass);
			Assert.AreEqual("test", age.CssClass);
			Assert.AreEqual("msg warning", label.CssClass);
			Assert.AreEqual("<ul>\r\n<li>Invalid Field Name The length should be 6 to 8 but was 9.</li>\r\n</ul>\r\n", label.Text);

			try {
				name.Text = "tom4567";
				age.Text = "";
				vh = new ValidateHandler();
				msg = new NoticeLabelAdapter(label, "notice", "warning");
				obj = ctx.GetObject<User>(page, "Field [{0}] parse error{1}");
				ctx.ValidateSave(page, vh, obj, msg, "hello", "inWarning", () => obj.Save());
			} catch (WebControlException ex) {
				ctx.ResetInputCss(page, "inWarning");
				PageHelper.SetCtrlClass(ex.RelatedControl, "inWarning");
				msg.AddMessage(ex.Message);
				msg.ShowWarning();
			}
			Assert.AreEqual("test", name.CssClass);
			Assert.AreEqual("test inWarning", age.CssClass);
			Assert.AreEqual("msg warning", label.CssClass);
			Assert.AreEqual("<ul>\r\n<li>Field [TestField] parse error</li>\r\n</ul>\r\n", label.Text);

			name.Text = "tom4567";
			age.Text = "20";
			vh = new ValidateHandler();
			obj = ctx.GetObject<User>(page, "parse error");
			msg = new NoticeLabelAdapter(label, "notice", "warning");
			ctx.ValidateSave(page, vh, obj, msg, "hello", "inWarning", () => obj.Save());
			Assert.AreEqual("test", name.CssClass);
			Assert.AreEqual("test", age.CssClass);
			Assert.AreEqual("msg notice", label.CssClass);
			Assert.AreEqual("<ul>\r\n<li>hello</li>\r\n</ul>\r\n", label.Text);
		}
	}
}

