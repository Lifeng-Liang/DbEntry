using System;
using System.Collections.Generic;
using Leafing.Core.Logging;

namespace Leafing.Core.TimingTask
{
    public class QueueTaskController : ServiceBase
    {
        protected readonly int RunPerMilliSecends;
        protected readonly List<ITask> Tasks = new List<ITask>();

        public QueueTaskController(int runPerMilliSecends)
        {
            if (runPerMilliSecends <= 0)
            {
                throw new ArgumentOutOfRangeException("runPerMilliSecends", "runPerMilliSecends should large than 0");
            }
            RunPerMilliSecends = runPerMilliSecends;
        }

        public void AddTask(ITask task)
        {
            Tasks.Add(task);
        }

        protected override void OnStarting()
        {
        }

        protected override void OnStopping()
        {
        }

        protected override void RunControllerTask()
        {
            foreach (var task in Tasks)
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

        protected void RoundSleep()
        {
            for(int i = RunPerMilliSecends; i > 0; i -= 1000)
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
