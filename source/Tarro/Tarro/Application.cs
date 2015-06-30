using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Threading;
using Tarro.Logging;

namespace Tarro
{
    internal class Application : IDisposable
    {
        private readonly ILog log = LogFactory.GetLogger<Application>();
        private readonly string name;
        private readonly string pathToApp;
        private readonly string executable;
        private readonly AppWatcher watcher;
        private AppDomain appDomain;
        public Application(string name, string pathToApp, string executable)
        {
            this.name = name;
            this.pathToApp = pathToApp;
            this.executable = executable;
            watcher = new AppWatcher(pathToApp);
            watcher.AppChanged += (o, e) =>
            {
                Stop();
                Start();
            };
        }

        public void Start()
        {
            try
            {
                log.Info("Starting application ({0})", name);
                var setup = new AppDomainSetup();
                CreateSetup(setup);

                appDomain = AppDomain.CreateDomain(setup.ApplicationName, new Evidence(), setup);
                appDomain.ExecuteAssembly(Path.Combine(pathToApp, executable));

                log.Info("Application started ({0})", name);
            }
            catch (Exception ex)
            {
                log.Error("Unable to start application ({0})", ex, name);
            }
        }

        private void CreateSetup(AppDomainSetup setup)
        {
            setup.ApplicationBase = pathToApp;
            setup.ApplicationName = Path.GetDirectoryName(pathToApp);
            setup.PrivateBinPath = pathToApp;
            setup.CachePath = Path.Combine(pathToApp, "Cache");
            setup.ShadowCopyFiles = "true";
            var potentialConfigFile = executable + ".config";
            if (File.Exists(Path.Combine(pathToApp, potentialConfigFile)))
                setup.ConfigurationFile = potentialConfigFile;
        }

        private void Stop()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }


        public void Dispose()
        {
            watcher.Dispose();
            if (appDomain != null)
            {
                Stop();
            }
        }

        public string Name { get { return name; } }
    }

}