using Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peperino_Api.Models.User
{
    [GenerateMapper]
    [AdaptTwoWays("[name]Dto")]
    public class User
    {
        [AdaptIgnore]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = new ObjectId();

        [BsonRequired]
        [BsonElement("username")]
        public string Username { get; set; } = "";

        [BsonRequired]
        [BsonElement("externalId")]
        public string ExternalId { get; set; } = "";
    }
}
