using System.ServiceProcess;
using Tarro.Configuration;
using Tarro.Logging;

namespace Tarro
{
    class ApplicationService : ServiceBase
    {
        private readonly Container container;
        public ApplicationService()
        {
            container = new Container();
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
        protected override void OnStart(string[] args)
        {
            container.Start();
        }

        protected override void OnStop()
        {
            container.Dispose();
        }

    }
}
