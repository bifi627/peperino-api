using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Peperino_Api.Models;

namespace Peperino_Api.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IOptions<MongoSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(mongoSettings.Value.UsersCollectionName);
        }

        public IEnumerable<User> GetAll()
        {
            return _usersCollection.Find(_ => true).ToEnumerable();
        }

        public Task<User> GetById(string id)
        {
            return _usersCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }
        public Task CreateAsync(User user)
        {
            return _usersCollection.InsertOneAsync(user);
        }

        public Task UpdateAsync(string id, User updatedUser)
        {
           return _usersCollection.ReplaceOneAsync(x => x.Id.ToString() == id, updatedUser);
        }

        public Task RemoveAsync(string id)
        {
            return _usersCollection.DeleteOneAsync(x => x.Id.ToString() == id);
        }

        public Task<User> GetByExternalId(string externalId)
        {
            return _usersCollection.Find(x => x.ExternalId == externalId).FirstOrDefaultAsync();
        }
    }
}
