using System.ServiceProcess;
using Forge.AutoUpdate.Client.Properties;
using Forge.AutoUpdate.Scheduler;

namespace Forge.AutoUpdate.Client
{
    class WindowsService : ServiceBase
    {
        readonly UpdateScheduler updateScheduler;

        internal WindowsService()
        {
            ServiceName = Settings.Default.ServiceName;
            updateScheduler = new UpdateScheduler();
        }

        protected override void OnStart(string[] args)
        {
            updateScheduler.Start();
        }

        protected override void OnPause()
        {
            updateScheduler.Stop();
        }

        protected override void OnContinue()
        {
            updateScheduler.Start();
        }

        protected override void OnStop()
        {
            updateScheduler.Stop();
        }
    }
}