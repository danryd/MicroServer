using System;
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
    class RouteHandler : Handler
    {
        private readonly RouteOptions options;
        public RouteHandler(Handler next, RouteOptions options) : base(next)
        {
            this.options = options;
        }

        public override async Task Handle(HttpListenerContext context)
        {
            if (options.Route == context.Request.Url.AbsolutePath)
            {
               
                var output =await options.Handler(context);
                await ReturnResponse(context, 200, output);
            }
               
            else
                await next.Handle(context);
        }
    }
    class RouteOptions
    {
        public string Route { get;private set; }
        public Func<HttpListenerContext, Task<string>> Handler { get; private set; }

        public RouteOptions(string route, Func<HttpListenerContext, Task<string>> handler  )
        {
            Route = route;
            Handler = handler;
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
