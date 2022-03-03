using System;
using System.Net;
using System.Net.Http;

namespace ais_erp_proxy.Models
{
    public class RequestWrapper
    {
        public static HttpResponseMessage DoSomethingWithRequest(HttpRequestMessage req)
        {
            try
            {
                ReceivedRequestData parsed_data = CustomRequsetInspector.CheckAndGetDataFromRequest(req);
                if (!parsed_data.status)
                {
                    return req.CreateResponse<ReceivedRequestData>(HttpStatusCode.Ambiguous, parsed_data);
                }
                else
                {
                    ReceivedResponseJson received_response = CustomRequestHandler.MakeRequest(parsed_data);
                    return req.CreateResponse<ReceivedResponseJson>(HttpStatusCode.OK, received_response);
                }
            }
            catch (Exception e)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, e.ToString());
            }
        }
    }
}