using System;

namespace Tarro.Management
{
    interface IRouter
    {
        Func<Response> Route(Uri uri);
    }
}
