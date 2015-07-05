using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class HttpServer : IDisposable
    {
        private HttpListener listener;

        private Dictionary<string, Func<HttpListenerRequest,string>> routeTable;
        public HttpServer( Dictionary<string, Func<HttpListenerRequest, string>> routes)
        {
            routeTable = routes;
            var uri = new Uri("http://localhost:2250");
            listener = new HttpListener();
            listener.Prefixes.Add(uri.ToString());
            listener.Start();
            Task.Run(() => Receive());
        }

        private async Task Receive()
        {
            while (listener.IsListening)
            {
                var ctx = await listener.GetContextAsync();
                string content = string.Empty;
                if (routeTable.ContainsKey(ctx.Request.Url.AbsolutePath))
                {
                    content = routeTable[ctx.Request.Url.AbsolutePath](ctx.Request
                        );
                    ctx.Response.StatusCode = 200;

                }
                else
                {
                    content = "404 not found";
                    ctx.Response.StatusCode = 404;

                }
                byte[] output = Encoding.UTF8.GetBytes(content);
                ctx.Response.OutputStream.Write(output, 0, output.Length);
                ctx.Response.OutputStream.Close();
            }

        }


        public void Dispose()
        {
            listener.Close();
        }
    }
}
