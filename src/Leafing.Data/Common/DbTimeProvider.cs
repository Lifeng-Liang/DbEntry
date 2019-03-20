using System;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Common {
    public class DbTimeProvider {
        private readonly DataProvider _context;
        private DateTime _lastCheckTime;
        private TimeSpan _timeDiff;

        public DbTimeProvider(DataProvider context) {
            _context = context;
        }

        public DateTime Now {
            get {
                var now = Util.Now;
                if ((now - _lastCheckTime).TotalMinutes > ConfigReader.Config.Database.DbTimeCheckMinutes) {
                    _lastCheckTime = now;
                    var dbNow = _context.GetDatabaseTime();
                    _timeDiff = now - dbNow;
                }
                return now - _timeDiff;
            }
        }
    }
}