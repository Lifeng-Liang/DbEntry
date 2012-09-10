using System;
using System.IO;
using System.Text;
using System.Threading;
using Leafing.Core.Ioc;

namespace Leafing.Core.Logging
{
    [Implementation("CacheTextFile")]
    public class CacheTextFileRecorder : TextFileLogRecorder
    {
        private readonly object _syncRoot = new object();
        protected Timer Timer;
        protected StringBuilder Holder;

        public CacheTextFileRecorder()
        {
            Holder = new StringBuilder();
            Timer = new Timer(TimeUp, null, 10000, 10000);
        }

        private void TimeUp(object state)
        {
            try
            {
                StringBuilder sb;
                lock (_syncRoot)
                {
                    sb = Holder;
                    Holder = new StringBuilder();
                }

                if (sb.Length > 0)
                {
                    using (var sw = new StreamWriter(GetLogFileName(), true, Encoding.UTF8))
                    {
                        sw.Write(sb.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                Util.CatchAll(() => Logger.System.Error(ex));
            }
        }

        public override void ProcessLog(SysLogType type, string name, string message, Exception exception)
        {
            lock (_syncRoot)
            {
                Holder.AppendFormat("{0}|{1}|{2}|{3}|{4}\n", type, name, message, exception, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
        }

        ~CacheTextFileRecorder()
        {
            TimeUp(null);
        }
    }
}
