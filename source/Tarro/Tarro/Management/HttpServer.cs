using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class HttpServer : IDisposable
    {
        private readonly IRouter router;
        private readonly HttpListener listener;

        public HttpServer(string uri, IRouter router)
        {
            this.router = router;

            var baseUri = new Uri(uri);
            listener = new HttpListener();
            listener.Prefixes.Add(baseUri.ToString());
            listener.Start();
            Task.Run(() => Receive());
        }

        private async Task Receive()
        {
            while (listener.IsListening)
            {
                var ctx = await listener.GetContextAsync();
                Exception ex = null;
                try
                {
                    //    var handler = router.Route(ctx.Request.Url);
                    //    var response = handler();

                    //    await WriteResponse(ctx, response.HttpStatusCode, response.Content, response.ContentType);
                    if (ctx.Request.Url.PathAndQuery.StartsWith("/nf"))
                        ctx.Response.StatusCode = 404;
                    else
                        ctx.Response.StatusCode = 200;
                    ctx.Response.Close();

                }
                catch (Exception e)
                {
                    ex = e;
                }
                if (ex != null)
                    await WriteResponse(ctx, 500, "Internal Server Error", "text/plain");
            }

        }

        protected async Task WriteResponse(HttpListenerContext context, int statusCode, string content, string contentType)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;
            var bytes = Encoding.UTF8.GetBytes(content);
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            context.Response.Close();
        }
        public void Dispose()
        {
            listener.Close();
        }
    }
}
