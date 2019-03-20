using System.Threading;

namespace Leafing.Core.TimingTask {
    public class ThreadingTimerAdapter : ITimer {
        protected TimerCallback tcb;
        protected Timer mTimer;
        protected bool mEnabled;
        protected double mInterval;
        public event System.Timers.ElapsedEventHandler Elapsed;

        public ThreadingTimerAdapter() : this(1000) { }

        public ThreadingTimerAdapter(double interval) {
            tcb = Timer_Elapsed;
            mInterval = interval;
            mTimer = new Timer(tcb, null, (long)mInterval, (long)mInterval);
        }

        private void Timer_Elapsed(object obj) {
            if (Elapsed != null && mEnabled) {
                Elapsed(this, null);
            }
        }

        public bool Enabled {
            get { return mEnabled; }
            set { mEnabled = value; }
        }

        public double Interval {
            get {
                return mInterval;
            }
            set {
                mInterval = value;
                mTimer = new Timer(tcb, null, (long)mInterval, (long)mInterval);
            }
        }

        public void Start() {
            Enabled = true;
        }

        public void Stop() {
            Enabled = false;
        }
    }
}
