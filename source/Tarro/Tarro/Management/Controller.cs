using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
   abstract class Controller
    {
       protected async Task<String> Json(object obj)
       {
           return await Content("to string");

       }
        protected abstract  Task<String> Content(string output);

    }
}
