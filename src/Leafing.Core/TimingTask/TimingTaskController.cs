using System;
using System.Timers;
using Leafing.Core.Logging;

namespace Leafing.Core.TimingTask {
    public class TimingTaskController : IDisposable {
        private readonly ITimer _timer;
        private readonly TimingTaskCollection _tasks;
        private bool _starting;

        public TimingTaskController(TimingTaskCollection tasks)
            : this(tasks, new ThreadingTimerAdapter(1000)) { }

        public TimingTaskController(TimingTaskCollection tasks, ITimer it) {
            _starting = false;
            this._tasks = tasks;
            _timer = it;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e) {
            foreach (TimingTask t in _tasks) {
                try {
                    t.RunIfTimingUp();
                } catch (Exception ex) {
                    Logger.Default.Error(ex);
                }
            }
        }

        public void Start() {
            if (!_starting) {
                _starting = true;
                _timer.Elapsed += TimerElapsed;
                _timer.Start();
            }
        }

        public void Dispose() {
            if (_starting) {
                _starting = false;
                _timer.Elapsed -= TimerElapsed;
                _timer.Stop();
            }
        }
    }
}
