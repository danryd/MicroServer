using System;
using System.IO;
using System.Diagnostics;
using System.Timers;
using Tarro.Logging;

namespace Tarro
{

    internal class AppWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;
        private readonly ILog log = LogFactory.GetLogger<AppWatcher>();
        private readonly Timer timer;
        public AppWatcher(string path, double timeoutInSeconds = 1)
        {
            watcher = new FileSystemWatcher(path);
            watcher.EnableRaisingEvents = true;
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            timer = new Timer(CalculateTimeoutInMs(timeoutInSeconds));
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;

        }

        private static double CalculateTimeoutInMs(double timeoutInSeconds)
        {
            return timeoutInSeconds * 1000;
        }


        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            log.Verbose($"Item renamed {e.OldName} -> {e.Name}");
            OnAppChanged(e.OldName);
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            log.Verbose($"Item deleted {e.Name}");
            OnAppChanged(e.Name);
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            log.Verbose($"Item created {e.Name}");
            OnAppChanged(e.Name);
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            log.Verbose($"Item changed {e.Name}");
            OnAppChanged(e.Name);
        }

        public event EventHandler<AppChangedEventHandlerArgs> AppChanged;

        public event EventHandler<AfterQuietPeriodEventArgs> AfterQuietPeriod;

        protected virtual void OnAppChanged(string name)
        {
            var lowerName = name.ToLower();
            if (IsCodeOrConfig(lowerName))
            {
                ResetTimer();

                var handler = AppChanged;
                if (handler != null) handler(this, new AppChangedEventHandlerArgs());
            }
        }

        private static bool IsCodeOrConfig(string lowerName)
        {
            return lowerName.EndsWith(".config") || lowerName.EndsWith(".exe") || lowerName.EndsWith(".dll");
        }

        private void ResetTimer()
        {
            log.Verbose($"Resetting timer");
            timer.Stop();
            timer.Start();

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnAfterQuietPeriod();
        }

        protected virtual void OnAfterQuietPeriod()
        {
            log.Verbose($"Quiet period ended");
            var handler = AfterQuietPeriod;
            if (handler != null) handler(this, new AfterQuietPeriodEventArgs());
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }

    internal class AfterQuietPeriodEventArgs
    {
    }

    internal class AppChangedEventHandlerArgs
    {
    }
}
