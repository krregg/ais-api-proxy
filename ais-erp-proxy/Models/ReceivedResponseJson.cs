namespace ais_erp_proxy.Models
{
    public class ReceivedResponseJson
    {
        public bool came_through { get; set; }
        public string response_status { get; set; }
        public string content { get; set; }
        public string contenttype { get; set; }
    }
}