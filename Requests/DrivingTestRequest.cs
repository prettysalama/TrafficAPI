using TrafficAPI.Models;

namespace TrafficAPI.Requests
{
    public class DrivingTestRequest
    {
        public string   CitizenId { get; set; }
        public DateTime TestDay   { get; set; }
        public VisaCard VisaCard  { get; set; }
    }
}
