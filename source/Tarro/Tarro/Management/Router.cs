using System;
using System.Collections.Generic;

namespace Tarro.Management
{
    class Router:IRouter
    {
        private readonly Uri baseUri;
        private readonly Dictionary<Uri, Func<Response>> routeActions = new Dictionary<Uri, Func<Response>>();

        public Router(Uri baseUri)
        {
            this.baseUri = baseUri;
            routeActions.Add(new Uri(baseUri,"/"), () => { return new Response { HttpStatusCode = 200,ContentType = "text/plain", Content = "" }; });
        }

        public Func<Response> Route(Uri uri)
        {
            foreach (var route in routeActions)
            {
             
                if (uri.MakeRelativeUri(route.Key).AbsolutePath=="")
                {
                    return route.Value;

                }
            }
            return ()=> new Response{HttpStatusCode = 404, Content = "Not found", ContentType = "text/plain"};
        }
    }
}