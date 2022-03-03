namespace ais_erp_proxy.Models
{
    public class ReceivedRequestData
    {
        public bool status { get; set; }
        public string info { get; set; }
        public string endpoint { get; set; }
        public string soapaction { get; set; }
        public string contenttype { get; set; }
        public string content { get; set; }
        public string type { get; set; }
    }
}