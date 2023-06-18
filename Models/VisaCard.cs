namespace TrafficAPI.Models
{
    public class VisaCard
    {
        public string   CardNumber     { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string   CVV            { get; set; }
    }
}
