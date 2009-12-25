using System;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public class TimeValue
    {
        public DateTime ExpiredOn;
        public object Value;

        public TimeValue(DateTime expiredOn, object value)
        {
            this.ExpiredOn = expiredOn;
            this.Value = value;
        }

        public static DateTime GetExpiredOn()
        {
            return MiscProvider.Instance.Now.AddMinutes(DataSetting.CacheMinutes);
        }

        public static TimeValue CreateTimeValue(object value)
        {
            return new TimeValue(GetExpiredOn(), value);
        }
    }
}
