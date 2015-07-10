using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class RoutingHandler:Handler
    {
        private Routes routes = new Routes();
        public RoutingHandler(Handler next):base(next)
        {
                
        }
        public override async Task Handle(System.Net.HttpListenerContext context)
        {
            if (routes.IsConfigured(context))
               await routes.Route(context);
            else
                await next.Handle(context);
        }
    }
}
