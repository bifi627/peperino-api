using Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Peperino_Api.Models.Abstractions;

namespace Peperino_Api.Models.Entity
{
    public class OwnableEntity<T>: IOwnable<T> where T : class
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = new ObjectId();

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ownerId")]
        public ObjectId OwnerId { get; set; } = new ObjectId();

        [BsonElement("content")]
#pragma warning disable CS8625 // Ein NULL-Literal kann nicht in einen Non-Nullable-Verweistyp konvertiert werden.
        public T Content { get; set; } = default;
#pragma warning restore CS8625 // Ein NULL-Literal kann nicht in einen Non-Nullable-Verweistyp konvertiert werden.
    }
}
