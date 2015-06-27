using System;
using System.IO;

namespace Tarro
{

    internal class AppWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;
        private DateTime lastChange;
        private TimeSpan timeout;
        public AppWatcher(string path, double timeoutInSeconds = 0.5)
        {
            timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            watcher = new FileSystemWatcher(path);
            watcher.EnableRaisingEvents = true;
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
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


        protected virtual void OnAppChanged()
        {
            if (DateTime.Now - lastChange > timeout)
            {
                lastChange = DateTime.Now;
                var handler = AppChanged;
                if (handler != null) handler(this, new AppChangedEventHandlerArgs());
            }
        }


        public void Dispose()
        {
            watcher.Dispose();
        }
    }

    internal class AppChangedEventHandlerArgs
    {
    }
}
