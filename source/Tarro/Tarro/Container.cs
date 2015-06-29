using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tarro.Configuration;
using Tarro.Logging;

namespace Tarro
{
    internal class Container : IDisposable
    {
        private ILog log = LogFactory.GetLogger<Container>();
        private readonly List<ApplicationThread> applications;
        public Container()
        {
            applications = new List<ApplicationThread>();
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

        }

        public void Dispose()
        {
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

        private class ApplicationThread:IDisposable
        {
            private readonly Application application;
            public object Name { get { return application.Name; }}

            private Thread thread;
            public ApplicationThread(Application application)
            {
                this.application = application;
            }

            internal void Start()
            {
                thread = new Thread(()=> application.Start());
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
