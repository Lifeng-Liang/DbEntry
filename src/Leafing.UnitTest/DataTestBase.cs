using Leafing.Core;
using Leafing.Core.Ioc;
using Leafing.Core.Logging;
using Leafing.Data;
using Leafing.Data.Definition;
using System;
using System.Data.SQLite;

namespace Leafing.UnitTest {
    public class DataTestBase : SqlTestBase {
        private const string FileName = "UnitTest.db";
        private static readonly string TestFilePath = "/Volumes/RamDisk/" + FileName;
        private static readonly byte[] TestFileBuffer = ResourceHelper.ReadAll(typeof(DataTestBase), FileName);

        protected override void OnSetUp() {
            System.Data.SQLite.TypeHolder.Type = StreamType.Memory;
            var bs = new byte[TestFileBuffer.Length];
            TestFileBuffer.CopyTo(bs, 0);
            System.Data.SQLite.MemFileStream.Files[TestFilePath] = bs;
        }

        protected override void OnTearDown() {
            System.Data.SQLite.MemStreamHandler.Instance.Delete(TestFilePath);
        }
    }

    [DisableSqlLog]
    public class LeafingLog : DbObjectModel<LeafingLog> {
        public LogLevel Type { get; set; }
        public string Thread { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        [SpecialName] public DateTime CreatedOn { get; set; }

        public LeafingLog() { }

        public LeafingLog(LogLevel type, string name, string message, Exception exception) {
            this.Type = type;
            this.Thread = GetThreadInfo();
            this.Name = name;
            this.Message = message;
            this.Exception = (exception == null) ? "" : exception.ToString();
        }

        private static string GetThreadInfo() {
            return GetThreadInfo(System.Threading.Thread.CurrentThread);
        }

        private static string GetThreadInfo(System.Threading.Thread t) {
            return string.Format("<{0}>{1},{2},{3},{4}",
                t.Name,
                t.GetApartmentState(),
                t.Priority,
                t.IsThreadPoolThread,
                t.IsBackground
                );
        }
    }

    [Implementation("Database")]
    public class DatabaseLogRecorder : ILogRecorder {
        public void ProcessLog(LogLevel type, string name, string message, Exception exception) {
            var li = new LeafingLog(type, name, message, exception);
            try {
                DbEntry.NewTransaction(() => DbEntry.Save(li));
            } catch (Exception ex) {
                string msg = (exception == null) ? message : message + "\n" + exception;
                if (Logger.System.LogRecorders != null) {
                    foreach (var recorder in Logger.System.LogRecorders) {
                        recorder.ProcessLog(type, name, msg, ex);
                    }
                }
            }
        }
    }
}