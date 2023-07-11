using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficAPI.Models
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ID { get; set; }

        public string Type { get; set; }
        public decimal Price { get; set; }
        public string CarPlateNumber { get; set; }
    }
}
