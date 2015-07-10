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

        public Dictionary<string, Func<HttpListenerContext,Task>> ConfiguredRoutes
        {
            get
            {
                return new Dictionary<string, Func<HttpListenerContext,Task>>
        {
            {"/", (ctx)=>  Write("Root", ctx)},
            {"/app",(ctx)=>Write("App",ctx)}
        };
            }
        }

        private async Task Write(string message, HttpListenerContext ctx)
        {
            ctx.Response.StatusCode = 200;
            var bytes = Encoding.UTF8.GetBytes(message);
            await ctx.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            ctx.Response.Close();

        }
        internal bool IsConfigured(HttpListenerContext context)
        {
            return ConfiguredRoutes.ContainsKey(context.Request.Url.AbsolutePath);
        }

        internal Task Route(HttpListenerContext context)
        {
            return Task.Run(async ()=> ConfiguredRoutes[context.Request.Url.AbsolutePath](context));
        }
    }
}
