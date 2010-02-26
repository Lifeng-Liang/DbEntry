using System;
using System.Threading;
using Lephone.Util.Setting;

namespace Lephone.Util
{
    public class MiscProvider
    {
        public static readonly MiscProvider Instance = (MiscProvider)ClassHelper.CreateInstance(UtilSetting.MiscProvider);

        protected MiscProvider()
        {
            _secends = -1;
        }

        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }

        public virtual Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        private Timer _timer;
        private long _secends;

        public virtual long Secends
        {
            get
            {
                if (_secends < 0)
                {
                    _secends = 0;
                    _timer = new Timer(o =>
                              {
                                  _secends++;
                              }, null, 1000, 1000);
                }
                return _secends;
            }
        }
    }
}