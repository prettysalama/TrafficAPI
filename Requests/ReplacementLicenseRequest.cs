using TrafficAPI.Models;

namespace TrafficAPI.Requests
{
    public class ReplacementLicenseRequest
    {
        public string   CitizenId { get; set; }
        public VisaCard VisaCard  { get; set; }
    }
}
