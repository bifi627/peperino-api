using MongoDB.Bson;
using Peperino_Api.Models;
using Peperino_Api.Models.User;

namespace Peperino_Api.Services
{
    public interface IUserManagementService
    {
        public IEnumerable<User> GetAll();
        public Task<User> GetById(ObjectId id);
        public Task<User> GetByExternalId(string externalId);
        public Task<User> GetByUsername(string username);
        public Task CreateAsync(User user);
        public Task UpdateAsync(ObjectId id, User updatedUser);
        public Task RemoveAsync(ObjectId id);
        public Task<bool> Exists(User user);
        public Task<bool> CheckUsername(string username);
        public Task<User> CreateNewUser(User user, string email, string password);
        public Task<User> HandleProviderLogin(User user);
    }
}
