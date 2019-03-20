using Leafing.Core.TimingTask;

namespace Leafing.UnitTest.util.timingTask {
    public class MockTimer : ITimer {
        public event System.Timers.ElapsedEventHandler Elapsed;

        public bool Enabled {
            get { return false; }
            set { }
        }

        public double Interval {
            get { return 0; }
            set { }
        }

        public void Start() { }

        public void Stop() { }

        public void RaiseElapsed() {
            Elapsed?.Invoke(null, null);
        }
    }
}