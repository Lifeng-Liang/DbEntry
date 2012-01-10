using System;
using System.Threading;
using Leafing.Core.Logging;
using Leafing.Core.Setting;

namespace Leafing.Core
{
    public abstract class ServiceBase
    {
        protected Thread Controller;
        protected bool Running;

        public void Start()
        {
            Running = true;
            OnStarting();
            Controller = new Thread(ControllerThread);
            Controller.Start();
        }

        protected abstract void OnStarting();

        public void Stop()
        {
            OnStopping();
            Running = false;
        }

        protected abstract void OnStopping();

        private void SleepIfJustStarted()
        {
            var n = Util.SystemRunningMillisecends;
            if (n < CoreSettings.MinStartTicks)
            {
                for (int i = 0; i < CoreSettings.DelayToStart; i += 1000)
                {
                    Util.Sleep(1000);
                    if (!Running)
                    {
                        break;
                    }
                }
            }
        }

        private void ControllerThread()
        {
            SleepIfJustStarted();
            while (Running)
            {
                try
                {
                    RunControllerTask();
                }
                catch (Exception ex)
                {
                    Logger.Default.Error(ex);
                }
            }
        }

        protected abstract void RunControllerTask();
    }
}
