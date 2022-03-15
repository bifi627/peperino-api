using MongoDB.Bson;
using Peperino_Api.Models;
using Peperino_Api.Models.User;

namespace Peperino_Api.Services
{
    public interface IUserService
    {
        public IEnumerable<User> GetAll();
        public Task<User> GetById(ObjectId id);
        public Task<User> GetByExternalId(string externalId);
        public Task CreateAsync(User user);
        public Task UpdateAsync(ObjectId id, User updatedUser);
        public Task RemoveAsync(ObjectId id);
        public Task<bool> Exists(User user);
    }
}
