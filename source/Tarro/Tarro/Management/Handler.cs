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
    }
}
