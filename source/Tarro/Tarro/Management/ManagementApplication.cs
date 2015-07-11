using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class ManagementApplication : IDisposable
    {
        private readonly HandlerFactory handlerFactory;
        private HttpServer server;
        public ManagementApplication()
        {
            handlerFactory = new HandlerFactory();
            AddHandler<NotFoundHandler>();
        }
        public void AddHandler<T>(object options =null) where T : Handler
        {
            handlerFactory.Add(typeof(T),options);
        }

        public void Start()
        {
            var handlerPipeline = handlerFactory.CreatePipeline();
            server = new HttpServer(handlerPipeline);
           
        }


        public void Dispose()
        {
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }

       
    }
}
