using System;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace Tarro
{

    internal class AppWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;

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
            return timeoutInSeconds*1000;
        }


        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnAppChanged();
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            OnAppChanged();
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            OnAppChanged();
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnAppChanged();
        }

        public event EventHandler<AppChangedEventHandlerArgs> AppChanged;

        public event EventHandler<AfterQuietPeriodEventArgs> AfterQuietPeriod;

        protected virtual void OnAppChanged()
        {
            timer.Enabled = true;
            var handler = AppChanged;
            if (handler != null) handler(this, new AppChangedEventHandlerArgs());

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnAfterQuietPeriod();
        }

        protected virtual void OnAfterQuietPeriod()
        {
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
