
using MongoDB.Bson;

namespace Peperino_Api.Models.Abstractions
{
    public interface IOwnable<T>
    {   
        public ObjectId Id { get; set; }
        public ObjectId OwnerId { get; set; }
        public T Content { get; set; }
    }
}
