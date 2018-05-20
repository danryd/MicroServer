using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Tarro.Management;

namespace Tarro.Test.Management
{
    public class Consts
    {
        public static string MgmtHttpUri = "http://localhost:2250";
    }
    public class HttpServerTests
    {

        public void ThrowsOnInvalidUri()
        {
            Should.Throw<UriFormatException>(() => new HttpServer("bla", null));
        }
        public async Task CanGetReply()
        {
            using (var server = new HttpServer(Consts.MgmtHttpUri, new Router(new Uri(Consts.MgmtHttpUri))))
            {
                var c = new HttpClient();
                c.BaseAddress = new Uri(Consts.MgmtHttpUri);
                var result = await c.GetAsync("");
                result.StatusCode.ShouldBe(HttpStatusCode.OK);
            }
        }
        public async Task Returns404IfNotFound()
        {
            using (var server = new HttpServer(Consts.MgmtHttpUri, new Router(new Uri(Consts.MgmtHttpUri))))
            {

                var c = new HttpClient();
                c.BaseAddress = new Uri(Consts.MgmtHttpUri);
                var result = await c.GetAsync("/nf");
                result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            }
        }
    }
}
