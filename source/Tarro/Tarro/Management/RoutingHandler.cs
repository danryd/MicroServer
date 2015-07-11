using System.Net;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class RoutingHandler:Handler
    {
        private readonly Routes routes = new Routes();
        public RoutingHandler(Handler next):base(next)
        {
                
        }
        public override async Task Handle(HttpListenerContext context)
        {
            if (routes.IsConfigured(context))
               await routes.Route(context);
            else
                await next.Handle(context);
        }
    }
}
