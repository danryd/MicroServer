using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class FuncHandler : Handler
    {
        private readonly Func<Handler, HttpListenerContext, Task<Handler>> handlerFunc;

        public FuncHandler(Handler next,Func<Handler, HttpListenerContext, Task<Handler>> handlerFunc) : base(next)
        {
            this.handlerFunc = handlerFunc;
        }

       
        public override Task Handle(System.Net.HttpListenerContext context)
        {
             return handlerFunc(next, context);
        }
    }
}
