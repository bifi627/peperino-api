using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peperino_Api.Models
{
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = new ObjectId();

        [BsonElement("username")]
        public string Username { get; set; } = "";

        [BsonElement("externalId")]
        public string ExternalId { get; set; } = "";
    }
}
