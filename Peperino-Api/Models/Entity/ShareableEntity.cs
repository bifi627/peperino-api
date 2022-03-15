using Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Peperino_Api.Models.Abstractions;

namespace Peperino_Api.Models.Entity
{
    public class ShareableEntity<T> : OwnableEntity<T>, IShareable<T> where T : class
    {
        [BsonElement("sharedWith")]
        public List<ObjectId> SharedWith { get; set; } = new List<ObjectId>();
    }
}
