using System;
using System.Collections.Generic;
using Leafing.Core.Logging;

namespace Leafing.Core.TimingTask
{
    public class QueueTaskController : ServiceBase
    {
        private readonly int _runPerMilliSecends;
        private readonly List<ITask> _tasks = new List<ITask>();

        public QueueTaskController(int runPerMilliSecends)
        {
            if (runPerMilliSecends <= 0)
            {
                throw new ArgumentOutOfRangeException("runPerMilliSecends", "runPerMilliSecends should large than 0");
            }
            _runPerMilliSecends = runPerMilliSecends;
        }

        public void AddTask(ITask task)
        {
            _tasks.Add(task);
        }

        protected override void OnStarting()
        {
        }

        protected override void OnStopping()
        {
        }

        protected override void RunControllerTask()
        {
            foreach (var task in _tasks)
            {
                try
                {
                    task.Run();
                }
                catch(Exception ex)
                {
                    Logger.Default.Error(ex);
                }
            }
            RoundSleep();
        }

        private void RoundSleep()
        {
            for(int i = _runPerMilliSecends; i > 0; i -= 1000)
            {
                var n = i > 1000 ? 1000 : i;
                Util.Sleep(n);
                if(!Running)
                {
                    break;
                }
            }
        }
    }
}
