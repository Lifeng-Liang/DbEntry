using System.IO;
using Leafing.Core;

namespace Leafing.UnitTest
{
    public class DataTestBase : SqlTestBase
    {
        private const string FileName = "UnitTest.db";
        private static readonly string TestFilePath = SystemHelper.TempDirectory + FileName;
        private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(DataTestBase), FileName);

        protected override void OnSetUp()
        {
            File.Delete(TestFilePath);
            using (Stream s = new FileStream(TestFilePath, FileMode.Create))
            {
                s.Write(TestFileBuffer, 0, TestFileBuffer.Length);
            }
        }

        protected override void OnTearDown()
        {
            File.Delete(TestFilePath);
        }
    }
}
