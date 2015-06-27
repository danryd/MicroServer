using System.ServiceProcess;
using Tarro.Logging;

namespace Tarro
{
    class ApplicationService : ServiceBase
    {
        private readonly ILog log = LogFactory.GetLogger<ApplicationService>();
        private Application app;
        public ApplicationService()
        {
            app = new Application(ServerSettings.Settings.PathToApp, ServerSettings.Settings.Executable);
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
        protected override void OnStart(string[] args)
        {
            app.Start();
        }

        protected override void OnStop()
        {
            app.Dispose();
        }

    }
}
