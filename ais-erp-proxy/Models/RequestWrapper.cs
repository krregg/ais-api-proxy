using System;
using System.Net;
using System.Net.Http;
using System.Text;

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
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(parsed_data.info, Encoding.UTF8, parsed_data.contenttype)
                    };

                }
                else
                {
                    ReceivedResponseJson received_response = CustomRequestHandler.MakeRequest(parsed_data);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(parsed_data.content, Encoding.UTF8, parsed_data.contenttype)
                    };
                }
            }
            catch (Exception e)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, e.ToString());
            }
        }
    }
}