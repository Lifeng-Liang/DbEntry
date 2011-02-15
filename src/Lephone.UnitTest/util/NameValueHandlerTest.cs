using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Lephone.Core;
using Lephone.Core.Setting;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class NameValueHandlerTest
    {
        [Test]
        public void Test1()
        {
            var h = new NameValueSectionHandler();
            string s = ResourceHelper.ReadToEnd(this.GetType(), "UnitTest.config.xml");
            using (var ms = new MemoryStream())
            {
                byte[] bs = Encoding.Default.GetBytes(s);
                ms.Write(bs, 0, bs.Length);
                ms.Flush();
                ms.Position = 0;
                var xd = new XmlDocument();
                xd.Load(ms);

                var config = xd["configuration"];
                Assert.IsNotNull(config);
                var l = (NameValueCollection)h.Create(null, null, config.ChildNodes[1]);
                Assert.AreEqual("Lephone.Core.Logging.ConsoleMessageLogRecorder, Lephone.Core | Lephone.UnitTest.SqlRecorder, Lephone.UnitTest", l["SqlLogRecorder"]);
                Assert.AreEqual("@Access : @~test.mdb", l["1.DataBase"]);
            }
        }
    }
}
