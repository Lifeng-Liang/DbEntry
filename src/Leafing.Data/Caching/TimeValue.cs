using System;
using Leafing.Core;
using Leafing.Core.Setting;

namespace Leafing.Data.Caching {
    public class TimeValue {
        public DateTime ExpiredOn;
        public object Value;

        public TimeValue(DateTime expiredOn, object value) {
            this.ExpiredOn = expiredOn;
            this.Value = value;
        }

        public static DateTime GetExpiredOn() {
            return Util.Now.AddSeconds(ConfigReader.Config.Database.Cache.KeepSecends);
        }

        public static TimeValue CreateTimeValue(object value) {
            return new TimeValue(GetExpiredOn(), value);
        }
    }
}
