using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Tarro.Management
{
    internal class HandlerFactory
    {
        private readonly Queue<Tuple<Type,object>> handlers = new Queue<Tuple<Type,object>>();
        public void Add(Type type, object options = null)
        {
            handlers.Enqueue(new Tuple<Type, object>(type,options));
        }

        internal Handler CreatePipeline()
        {
            Handler handler = null;
            while (handlers.Count > 0)
            {
                var handlerType = handlers.Dequeue();
                var parameters = CreateConstructorParameters(handlerType.Item1, handlerType.Item2, handler);
                handler = (Handler)Activator.CreateInstance(handlerType.Item1, parameters);

            }
            return handler;
        }

        private object[] CreateConstructorParameters(Type handlerType, object options, Handler handler)
        {
            var constructor = handlerType.GetConstructors().Single();
            if (constructor.GetParameters().Count() == 1)
                return new object[] { handler };
            return new object[]{handler,options};
        }
    }
}