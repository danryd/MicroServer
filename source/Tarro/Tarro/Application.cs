using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Tarro.Logging;

namespace Tarro
{
    enum RunMode
    {
        AppDomain, Process
    }

    internal class Application : MarshalByRefObject, IDisposable
    {

        private readonly ILog log = LogFactory.GetLogger<Application>();
        private readonly string name;
        private readonly AppWatcher watcher;
        private readonly AppCopy appCopy;

        private readonly RunMode runMode;
        private readonly string cachePath = "appCache";
        private readonly AppRuntime runtime;
        public Application(string name, string pathToApp, string executable, RunMode runMode = RunMode.AppDomain)
        {
            this.name = name;
            this.runMode = runMode;

            appCopy = new AppCopy(cachePath, pathToApp, executable);
            watcher = new AppWatcher(pathToApp);
            switch (runMode)
            {
                case RunMode.AppDomain:
                    runtime = new AppDomainRuntime(name, pathToApp, executable, appCopy.ShadowPath);
                    break;
                case RunMode.Process:
                    runtime = new ProcessRuntime(name, pathToApp, executable, appCopy.ShadowPath);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown runmode, {runMode}");
            }

            watcher.AppChanged += (o, e) =>
            {
                Stop();
            };
            watcher.AfterQuietPeriod += (o, e) =>
            {
                Start();
            };
        }

        private enum RuntimeState
        {
            Stopped, Starting, Running,
            Stopping
        }
        private RuntimeState state = RuntimeState.Stopped;
        public void Start()
        {
            try
            {
                lock (appLock)
                {
                    try
                    {
                        log.Info("Starting application ({0}), current state {1}", name, state);
                        if (state != RuntimeState.Stopped)
                            return;
                        state = RuntimeState.Starting;
                        if (runtime.IsActive()) //Should be null if stopped
                            return;

                        appCopy.ShadowCopy();
                        runtime.Start();

                        log.Info("Application started ({0})", name);
                        state = RuntimeState.Running;

                    }
                    catch (Exception)
                    {
                        state = RuntimeState.Stopped;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to start application ({0})", ex, name);
                state = RuntimeState.Stopped;
            }
        }

        private readonly object appLock = new object();
        private void Stop()
        {
            lock (appLock)
            {
                if (state != RuntimeState.Running)
                    return;
                state = RuntimeState.Stopping;

                runtime.Stop();
                log.Info("Application stopped ({0})", name);
                state = RuntimeState.Stopped;
            }
        }


        public void Dispose()
        {
            watcher.Dispose();
            runtime.Stop();
        }

        public string Name => name;
    }

}