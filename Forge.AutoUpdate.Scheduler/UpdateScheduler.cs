using System;
using System.Threading;
using Forge.AutoUpdate.Scheduler.Properties;

namespace Forge.AutoUpdate.Scheduler
{
    //TODO add update installation
    public class UpdateScheduler
    {
        Timer timer;

        public void Start()
        {
            timer = new Timer(
                new TimerCallback(state => new UpdateDownloader().DownloadLatestVersion()),
                null,
                TimeSpan.Zero,
                Settings.Default.CheckForUpdatesPeriod);
        }

        public void Stop()
        {
            timer.Dispose();
        }
    }
}