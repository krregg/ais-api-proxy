using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ais_erp_proxy.Models
{
    public class CustomRequestHandler
    {
        public static ReceivedResponseJson MakeRequest(ReceivedRequestData data)
        {
            try
            {
                ReceivedResponseJson rsp = new ReceivedResponseJson();

                string username = ConfigurationManager.AppSettings["ErpUsername"];
                string password = ConfigurationManager.AppSettings["ErpPassword"];
                string domain = ConfigurationManager.AppSettings["Domain"];

                var creds = new NetworkCredential(username, password, domain);
                var client = new HttpClient(new WinHttpHandler() { ServerCredentials = creds });

                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue(data.contenttype));

                if (data.contenttype == "application/xml")
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", data.soapaction);
                }

                if (data.type.ToLower() == "get")
                {
                    var response = client.GetAsync(data.endpoint).Result;

                    rsp.came_through = true;
                    rsp.contenttype = data.contenttype;
                    rsp.response_status = response.StatusCode.ToString();
                    rsp.content = response.Content.ReadAsStringAsync().Result;
                }
                if (data.type.ToLower() == "post")
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, data.endpoint);
                    requestMessage.Content = new StringContent(data.content, Encoding.UTF8, data.contenttype);
                    var response = client.SendAsync(requestMessage).Result;

                    rsp.came_through = true;
                    rsp.contenttype = data.contenttype;
                    rsp.response_status = response.StatusCode.ToString();
                    rsp.content = response.Content.ReadAsStringAsync().Result;
                }

                return rsp;
            }
            catch (Exception e)
            {
                ReceivedResponseJson rsp = new ReceivedResponseJson();
                rsp.came_through = false;
                rsp.response_status = "exception";
                rsp.content = e.ToString();
                return rsp;
            }
        }
    }
}