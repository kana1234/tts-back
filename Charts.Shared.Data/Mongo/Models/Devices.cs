using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Charts.Shared.Data.Mongo.Models
{
    [BsonIgnoreExtraElements]
    public class Devices
    {
        [BsonElement(nameof(device))]
        public string device { get; set; }
        [BsonElement(nameof(description))]
        public string description { get; set; }
    }
}
