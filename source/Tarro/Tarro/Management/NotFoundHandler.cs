using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class NotFoundHandler : Handler
    {


        public override async Task Handle(System.Net.HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            var bytes = Encoding.UTF8.GetBytes("Not found");
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            context.Response.Close();
         
        }

        public NotFoundHandler(Handler next) : base(next)
        {
        }
    }
}
