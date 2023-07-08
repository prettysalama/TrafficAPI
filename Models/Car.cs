using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace TrafficAPI.Models
{
    public class Car
    {
        [BsonId]
        public string   PlateNumber           { get; set; }
        public string   Model                 { get; set; }
        public string   Color                 { get; set; }
        public int      Year                  { get; set; }
        public int      Cylinders             { get; set; }
        public int      MotorCapacity         { get; set; }
        public DateTime CheckingDate          { get; set; }
        public DateTime LicenseCreationDate   { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
        public string   OwnerName             { get; set; }
    }
}
