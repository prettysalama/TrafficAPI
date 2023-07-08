using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace TrafficAPI.Models
{
    public class Citizen
    {
        [BsonId]
        public string   Id                  { get; set; }
        public string   DrivingLicenseType  { get; set; }
        public string   LicenseIssuanceUnit { get; set; }
        public DateTime DrivingTestDay      { get; set; }
        public VisaCard VisaCard            { get; set; }
    }
}
