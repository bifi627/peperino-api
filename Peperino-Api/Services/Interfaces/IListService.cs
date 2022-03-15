using MongoDB.Bson;
using Peperino_Api.Models;
using Peperino_Api.Models.User;

namespace Peperino_Api.Services
{
    public interface IListService
    {
        public Task<List?> GetById(User user, ObjectId id);
        public Task<string> Create(User user, List item);
    }
}
