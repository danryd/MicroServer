using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class ErrorHandler : Handler
    {
        public override async Task Handle(System.Net.HttpListenerContext context)
        {
            context.Response.StatusCode = 500;
            var bytes = Encoding.UTF8.GetBytes("Internal Server Error");
            await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            context.Response.Close();
         
        }

        public ErrorHandler(Handler next)
            : base(next)
        {
        }
    }
}