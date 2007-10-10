
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
