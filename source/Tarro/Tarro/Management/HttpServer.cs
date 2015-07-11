using System;
using System.Net;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class HttpServer : IDisposable
    {
        private readonly HttpListener listener;

        private readonly Handler handler;
        public HttpServer(Handler handler)
        {
            this.handler = handler;
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
                Exception ex = null;
                try
                {
                    await handler.Handle(ctx);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                if (ex != null)
                    await new ErrorHandler(null).Handle(ctx);
            }

        }


        public void Dispose()
        {
            listener.Close();
        }
    }
}
