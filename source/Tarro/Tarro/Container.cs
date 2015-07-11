using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tarro.Configuration;
using Tarro.Logging;
using Tarro.Management;

namespace Tarro
{
    internal class Container : IDisposable
    {
        private readonly ILog log = LogFactory.GetLogger<Container>();
        private readonly List<ApplicationThread> applications;
        private readonly ManagementApplication managementApplication;
        public Container()
        {
            applications = new List<ApplicationThread>();
            managementApplication = new ManagementApplication();
        }
        internal void Start()
        {
            foreach (var appElement in TarroSettings.Settings.Applications)
            {
                var application = new Application(appElement.Name, appElement.PathToApp, appElement.Executable);
                var appThread = new ApplicationThread(application);
                appThread.Start();
                applications.Add(appThread);
            }
            managementApplication.AddHandler<RoutingHandler>();
            managementApplication.Start();
        }

        public void Dispose()
        {
            try
            {
                managementApplication.Dispose();
            }
            catch (Exception ex)
            {

                log.Warn("Could not dispose http server.", ex);
            }
            foreach (var application in applications)
            {
                try
                {
                    application.Dispose();
                }
                catch (Exception ex)
                {
                    log.Warn("Could not dispose application {0}", ex, application.Name);
                }
            }

        }

        private class ApplicationThread : IDisposable
        {
            private readonly Application application;
            public object Name { get { return application.Name; } }

            private Thread thread;
            public ApplicationThread(Application application)
            {
                this.application = application;
            }

            internal void Start()
            {
                thread = new Thread(() => application.Start());
                thread.IsBackground = true;
                thread.Start();
            }

            public void Dispose()
            {
                application.Dispose();
                thread.Join(500);
                thread = null;
            }
        }
    }
}
