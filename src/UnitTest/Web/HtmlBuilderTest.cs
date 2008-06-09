using System;
using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class HtmlBuilderTest
    {
        [Test]
        public void Test1()
        {
            HtmlBuilder hb = HtmlBuilder.New
            .table
                .tr
                    .td
                        .text("test")
                    .end
                .end
            .end;
            string Act = hb.ToString();
            Assert.AreEqual("<table><tr><td>test</td></tr></table>", Act);
        }

        [Test, ExpectedException(typeof(WebException))]
        public void TestError()
        {
            HtmlBuilder hb = HtmlBuilder.New
            .table
                .tr
                    .td
                        .text("test")
                    .end
                .end;
            hb.ToString();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestErrorOverUseEnd()
        {
            HtmlBuilder hb = HtmlBuilder.New
            .table
                .tr
                    .td
                        .text("test")
                    .end
                .end
            .end
            .end;
        }

        [Test]
        public void Test2()
        {
            HtmlBuilder hb = HtmlBuilder.New.a("http://llf.javaeye.com").text("myblog").end;
            Assert.AreEqual("<a href=\"http://llf.javaeye.com\">myblog</a>", hb.ToString());
        }

        [Test]
        public void Test3()
        {
            HtmlBuilder b = HtmlBuilder.New.img("a.jpg");
            Assert.AreEqual("<img src=\"a.jpg\" />", b.ToString());
        }

        [Test]
        public void Test4()
        {
            HtmlBuilder b = HtmlBuilder.New.img("a.jpg", "abc", 96, 128);
            Assert.AreEqual("<img src=\"a.jpg\" alt=\"abc\" height=\"96\" width=\"128\" />", b.ToString());
        }

        [Test]
        public void Test5()
        {
            HtmlBuilder b = HtmlBuilder.New.a("t.aspx").text("tt").end;
            HtmlBuilder b2 = HtmlBuilder.New.li.include(b).end;
            Assert.AreEqual("<li><a href=\"t.aspx\">tt</a></li>", b2.ToString());
        }

        [Test]
        public void Test6()
        {
            HtmlBuilder b = HtmlBuilder.New.table.tr.td.Class("td").text("test").end.end.end;
            Assert.AreEqual("<table><tr><td class=\"td\">test</td></tr></table>", b.ToString());
        }

        [Test]
        public void Test7()
        {
            HtmlBuilder b = HtmlBuilder.New.li.Class("li").tag("xml").attr("ID", "xml").Class("it").text("test").end.end;
            Assert.AreEqual("<li class=\"li\"><xml ID=\"xml\" class=\"it\">test</xml></li>", b.ToString());
        }

        [Test]
        public void Test8()
        {
            HtmlBuilder b = HtmlBuilder.New.li.ul.end.end.li.end;
            Assert.AreEqual("<li><ul /></li><li />", b.ToString());
        }

        [Test]
        public void Test9()
        {
            HtmlBuilder b = HtmlBuilder.New.div.id("tt").text("test").end;
            Assert.AreEqual("<div id=\"tt\">test</div>", b.ToString());
        }

        [Test]
        public void Test10()
        {
            HtmlBuilder b = HtmlBuilder.New.div.div.div.end.end.end;
            Assert.AreEqual("<div><div><div /></div></div>", b.ToString());
        }

        [Test]
        public void Test11()
        {
            HtmlBuilder b = HtmlBuilder.New.div.div.div.text("test").end.end.end;
            Assert.AreEqual("<div><div><div>test</div></div></div>", b.ToString());
        }

        [Test]
        public void Test12()
        {
            HtmlBuilder b = HtmlBuilder.New.div.end.div.end;
            Assert.AreEqual("<div /><div />", b.ToString());
        }

        [Test]
        public void Test13()
        {
            HtmlBuilder b = HtmlBuilder.New.asp("TextBox", "mytb").end;
            Assert.AreEqual("<asp:TextBox ID=\"mytb\" runat=\"server\" />", b.ToString());
        }

        [Test]
        public void Test14()
        {
            HtmlBuilder b = HtmlBuilder.New.asp("DropDownList", "ddl").tag("asp:ListItem").attr("Text", "Work").end.end;
            Assert.AreEqual("<asp:DropDownList ID=\"ddl\" runat=\"server\"><asp:ListItem Text=\"Work\" /></asp:DropDownList>", b.ToString());
        }
    }
}
