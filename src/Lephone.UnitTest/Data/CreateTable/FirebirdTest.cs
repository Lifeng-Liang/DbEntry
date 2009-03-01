using System.Diagnostics;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.CreateTable
{
    public abstract class fbLongName : LinqObjectModel<fbLongName>
    {
        // use guid to gen index name and encode it into base32
        [Index]
        public abstract string N123456789012345678901234567890 { get; set; }
    }

    public abstract class fbLongName2 : LinqObjectModel<fbLongName2>
    {
        [Index]
        public abstract string Name { get; set; }
    }

    [TestFixture]
    public class FirebirdTest
    {
        [Test]
        public void TestToAvoidMoreThan31CharsIndexName()
        {
            var de = new DbContext("Firebird");
            de.Create(typeof(fbLongName));
            string s = getIndexName();
            Debug.Assert(s != null);
            Assert.AreEqual(29, s.Length);
            Assert.AreEqual("IX_1QCVK84BPESSPIBHJ1ND74RDE7", s);
        }

        private static string getIndexName()
        {
            string s = StaticRecorder.LastMessage.Substring(@"CREATE INDEX """.Length);
            int n = s.IndexOf("\"");
            s = s.Substring(0, n);
            return s;
        }

        [Test]
        public void TestForNormalIndexName()
        {
            var de = new DbContext("Firebird");
            de.Create(typeof(fbLongName2));
            string s = getIndexName();
            Debug.Assert(s != null);
            Assert.AreEqual("IX_FB_LONG_NAME2_NAME", s);
        }
    }
}
