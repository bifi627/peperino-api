using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Peperino_Api.Models.Abstractions
{
    public interface IShareable<T> : IOwnable<T>
    {
        public List<ObjectId> SharedWith { get; set; }
    }
}
