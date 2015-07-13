using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class RoutingHandler:Handler
    {
        private readonly RoutingOptions options;
        private readonly Routes routes = new Routes();
        public RoutingHandler(Handler next,RoutingOptions options ):base(next)
        {
            this.options = options;
        }

        public override async Task Handle(HttpListenerContext context)
        {
            if (routes.IsConfigured(context))
               await routes.Route(context);
            else
                await next.Handle(context);
        }
    }
    class RoutingOptions{
        private readonly IEnumerable<Application> applications;

        public RoutingOptions(IEnumerable<Application> applications)
        {
            this.applications = applications;
        }
    }
}
