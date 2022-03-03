﻿using System.Net.Http;

namespace ais_erp_proxy.Models
{
    public class CustomRequsetInspector
    {
        public static ReceivedRequestData CheckAndGetDataFromRequest(HttpRequestMessage req)
        {
            ReceivedRequestData rsp_json = new ReceivedRequestData();

            if (!req.Headers.Contains("type"))
            {
                rsp_json.status = false;
                rsp_json.info = "Content type is missing in your header";
                return rsp_json;
            }
            if (!req.Headers.Contains("Endpoint"))
            {
                rsp_json.status = false;
                rsp_json.info = "Endpoint is missing in your header";
                return rsp_json;
            }

            var hdr_enum = req.Headers.GetValues("type").GetEnumerator();
            hdr_enum.MoveNext();
            var content_type = hdr_enum.Current;

            if (content_type == "application/xml")
            {
                if (!req.Headers.Contains("Soap-Action"))
                {
                    rsp_json.status = false;
                    rsp_json.info = "soap action is missing in your header";
                    return rsp_json;
                }
            }
            else
            {
                if (content_type != "application/json")
                {
                    rsp_json.status = false;
                    rsp_json.info = "Invalid content type";
                    return rsp_json;
                }
            }

            rsp_json.status = true;
            hdr_enum = req.Headers.GetValues("Endpoint").GetEnumerator();
            hdr_enum.MoveNext();
            rsp_json.endpoint = hdr_enum.Current;
            rsp_json.contenttype = content_type;
            if (content_type == "application/xml")
            {
                rsp_json.soapaction = req.Headers.GetValues("Soap-Action").ToString();
                hdr_enum = req.Headers.GetValues("Soap-Action").GetEnumerator();
                hdr_enum.MoveNext();
                rsp_json.soapaction = hdr_enum.Current;
            }
            rsp_json.content = req.Content.ReadAsStringAsync().Result;
            rsp_json.type = req.Method.ToString();

            return rsp_json;
        }
    }
}