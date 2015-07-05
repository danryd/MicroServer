using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class Routes
    {

        public Dictionary<string, Func<HttpListenerRequest, string>> ConfiguredRoutes
        {
            get
            {
                return new Dictionary<string, Func<HttpListenerRequest, string>>
        {
            {"/", (request)=> "Root"},
            {"/app",(request)=> "App"}
        };
            }
        }
    }
}
