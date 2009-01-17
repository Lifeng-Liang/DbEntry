using System.IO;
using Lephone.MockSql.Recorder;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest
{
    public class DataTestBase
    {
        private const string FileName = "UnitTest.db";
        private static readonly string TestFilePath = SystemHelper.TempDirectory + FileName;
        private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(DataTestBase), FileName);

        [SetUp]
        public void SetUp()
        {
            File.Delete(TestFilePath);
            using (Stream s = new FileStream(TestFilePath, FileMode.Create))
            {
                s.Write(TestFileBuffer, 0, TestFileBuffer.Length);
            }
            StaticRecorder.ClearMessages();
            OnSetUp();
        }

        protected virtual void OnSetUp() { }

        [TearDown]
        public void TearDown()
        {
            File.Delete(TestFilePath);
        }
    }
}
