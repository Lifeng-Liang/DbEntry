using System;
using System.Threading;
using Lephone.Core.Setting;

namespace Lephone.Core
{
    public class MiscProvider
    {
        public static readonly MiscProvider Instance = (MiscProvider)ClassHelper.CreateInstance(CoreSettings.MiscProvider);

        protected MiscProvider() { }

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }

        public virtual Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        private static class TickProvider
        {
            // ReSharper disable UnaccessedField.Local
            // ReSharper disable NotAccessedField.Local
            private static Timer _timer; // to avoid it to be collected by GC.
            // ReSharper restore NotAccessedField.Local
            // ReSharper restore UnaccessedField.Local

            public static long Secends;

            static TickProvider()
            {
                _timer = new Timer(o => { Secends++; }, null, 1000, 1000);
            }
        }

        public virtual long Secends
        {
            get { return TickProvider.Secends; }
        }

        public virtual int SystemRunningMillisecends
        {
            get { return Environment.TickCount; }
        }

        public virtual void Sleep(int millisecends)
        {
            Thread.Sleep(millisecends);
        }
    }
}