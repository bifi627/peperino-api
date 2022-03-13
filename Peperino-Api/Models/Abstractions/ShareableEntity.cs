using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peperino_Api.Models.Abstractions
{
    public class ShareableEntity<T> : OwnableEntity<T>, IShareable<T> where T : class
    {
        [BsonElement("sharedWith")]
        public List<ObjectId> SharedWith { get; set; } = new List<ObjectId>();
    }
}
