using System.Diagnostics;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.CreateTable
{
    public abstract class fbLongName : DbObjectModel<fbLongName>
    {
        // use guid to gen index name and encode it into base32
        [Index]
        public abstract string N123456789012345678901234567890 { get; set; }
    }

    public abstract class fbLongName2 : DbObjectModel<fbLongName2>
    {
        [Index]
        public abstract string Name { get; set; }
    }

    public abstract class fbBlob : DbObjectModel<fbBlob>
    {
        [Length(64)]
        public abstract byte[] Blob1 { get; set; }

        [Length(85)]
        public abstract byte[] Blob2 { get; set; }

        public abstract byte[] Blob3 { get; set; }

        [Length(30)]
        public abstract string Name { get; set; }

        public abstract string Name2 { get; set; }
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
            StaticRecorder.ClearMessages();
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

        [Test]
        public void TestBlob()
        {
            var de = new DbContext("Firebird");
            de.Create(typeof(fbBlob));
            Assert.AreEqual(@"CREATE TABLE ""FB_BLOB"" (
    ""ID"" BIGINT NOT NULL PRIMARY KEY,
    ""BLOB1"" BLOB (64) NOT NULL ,
    ""BLOB2"" BLOB NOT NULL ,
    ""BLOB3"" BLOB SUB_TYPE 0 NOT NULL ,
    ""NAME"" VARCHAR (30) CHARACTER SET UNICODE_FSS NOT NULL ,
    ""NAME2"" BLOB SUB_TYPE 1 CHARACTER SET UNICODE_FSS NOT NULL
);
<Text><30>()".Replace("\r\n", "").Replace("    ", ""), StaticRecorder.Messages[0]);
        }
    }
}
