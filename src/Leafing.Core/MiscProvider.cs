using System;
using System.Threading;
using Leafing.Core.Ioc;

namespace Leafing.Core {
    [DependenceEntry, Implementation(1)]
    public class MiscProvider {
        public virtual DateTime Now {
            get { return DateTime.Now; }
        }

        public virtual Guid NewGuid() {
            return Guid.NewGuid();
        }

        private static class TickProvider {
            internal static Timer _timer;

            public static long Secends;

            static TickProvider() {
                _timer = new Timer(o => { Secends++; }, null, 1000, 1000);
            }
        }

        public virtual long Secends {
            get { return TickProvider.Secends; }
        }

        public virtual int SystemRunningMillisecends {
            get { return Environment.TickCount; }
        }

        public virtual void Sleep(int millisecends) {
            Thread.Sleep(millisecends);
        }

        public virtual DateTime ToLocalTime(DateTime time) {
            return time.ToLocalTime();
        }

        public virtual DateTime ToUniversalTime(DateTime time) {
            return time.ToUniversalTime();
        }
    }
}