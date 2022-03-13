using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peperino_Api.Models.Abstractions
{
    public class OwnableEntity<T>: IOwnable<T> where T : class
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = new ObjectId();

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ownerId")]
        public ObjectId OwnerId { get; set; } = new ObjectId();

        [BsonElement("content")]
        public T? Content { get; set; } = default;
    }
}
