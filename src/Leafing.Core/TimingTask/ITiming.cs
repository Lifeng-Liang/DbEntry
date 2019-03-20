using System;

namespace Leafing.Core.TimingTask {
    public interface ITiming {
        bool TimesUp();

        TimeSpan TimeSpanFromNowOn();

        void Reset();
    }
}
