using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Should;
using Tarro.Management;

namespace Tarro.Tests.Management
{
    class HandlerFactoryTests
    {
        private HandlerFactory factory;
        public HandlerFactoryTests()
        {
            factory = new HandlerFactory();
        }

        public void FactoryCreatesSingleItem()
        {
            factory.Add(typeof(NotFoundHandler));
            var pipeline = factory.CreatePipeline();
            pipeline.ShouldBeType(typeof (NotFoundHandler));
        }
        public void AddTwoItemsLastIsFirst()
        {
            factory.Add(typeof(NotFoundHandler));
            factory.Add(typeof(RoutingHandler));
            var pipeline = factory.CreatePipeline();
            pipeline.ShouldBeType(typeof(RoutingHandler));
        }
        public void AddTwoItemsLastIsNext()
        {
            factory.Add(typeof(NotFoundHandler));
            factory.Add(typeof(RoutingHandler));
            var pipeline = factory.CreatePipeline();
            pipeline.ShouldBeType(typeof(RoutingHandler));
        }

        public void HandlesOptions()
        {
            string options = "options";
            factory.Add(typeof(OptionHandler),options);
            var pipeline = factory.CreatePipeline() as OptionHandler;
            pipeline.ShouldNotBeNull();
            pipeline.Options.ShouldEqual("options");

        }
    }

    internal class OptionHandler : Handler
    {
        public object Options { get; private set; }

        public OptionHandler(Handler next, object options):base(next)
        {
            this.Options = options;
        }

        public override Task Handle(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
