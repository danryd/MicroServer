using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    internal abstract class Handler
    {
        protected readonly Handler next;

        protected Handler(Handler next)
        {
            this.next = next;
        }

        
        public abstract  Task Handle(HttpListenerContext context);
        public bool IsDone { get; set; }
        internal Handler Next { get{return next;} }

        protected static async Task ReturnResponse(HttpListenerContext context, int statusCode, string content)
        {
            context.Response.StatusCode = statusCode;
            var bytes = Encoding.UTF8.GetBytes(content);
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            context.Response.Close();
        }
    }
}
