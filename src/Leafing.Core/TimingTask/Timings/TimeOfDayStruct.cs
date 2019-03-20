using System;

namespace Leafing.Core.TimingTask.Timings {
    public class TimeOfDayStructure {
        private readonly TimeSpan _timeOfDay;

        public TimeOfDayStructure(int hour, int minute, int second) {
            _timeOfDay = new TimeSpan(hour, minute, second);
        }

        public TimeSpan TimeSpanFromMidNight {
            get { return _timeOfDay; }
        }
    }
}
