using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class ErrorHandler : Handler
    {
        public override async Task Handle(System.Net.HttpListenerContext context)
        {
            await ReturnResponse(context, 500, "Internal Server Error");
        }

        public ErrorHandler(Handler next)
            : base(next)
        {
        }
    }
}