using Peperino_Api.Models;

namespace Peperino_Api.Services
{
    public interface IUserService
    {
        public IEnumerable<User> GetAll();
        public Task<User> GetById(string id);
        public Task<User> GetByExternalId(string externalId);
        public Task CreateAsync(User user);
        public Task UpdateAsync(string id, User updatedUser);
        public Task RemoveAsync(string id);
    }
}
