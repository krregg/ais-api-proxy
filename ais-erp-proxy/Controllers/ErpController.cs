using System.Net.Http;
using System.Web.Http;
using ais_erp_proxy.Models;

namespace ais_erp_proxy.Controllers
{
    public class ErpController : ApiController
    {
        [BasicAuthentication]
        public HttpResponseMessage GetBcs()
        {
            return RequestWrapper.DoSomethingWithRequest(Request);
        }

        [BasicAuthentication]
        public HttpResponseMessage PostBcs()
        {
            return RequestWrapper.DoSomethingWithRequest(Request);
        }
    }
}