using System.IO;
using Leafing.Core;
using System.Data.SQLite;

namespace Leafing.UnitTest
{
    public class DataTestBase : SqlTestBase
    {
        private const string FileName = "UnitTest.db";
		private static readonly string TestFilePath = "/Volumes/RamDisk/" + FileName;
        private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(DataTestBase), FileName);

        protected override void OnSetUp()
        {
			System.Data.SQLite.TypeHolder.Type = StreamType.Memory;
			var bs = new byte[TestFileBuffer.Length];
			TestFileBuffer.CopyTo(bs, 0);
			System.Data.SQLite.MemFileStream.Files [TestFilePath] = bs;
        }

        protected override void OnTearDown()
        {
			System.Data.SQLite.MemStreamHandler.Instance.Delete (TestFilePath);
        }
    }
}
