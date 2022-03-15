using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Peperino_Api.Models;
using Peperino_Api.Models.User;

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

        public Task<User> GetById(ObjectId id)
        {
            return _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public Task CreateAsync(User user)
        {
            return _usersCollection.InsertOneAsync(user);
        }

        public Task UpdateAsync(ObjectId id, User updatedUser)
        {
           return _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);
        }

        public Task RemoveAsync(ObjectId id)
        {
            return _usersCollection.DeleteOneAsync(x => x.Id == id);
        }

        public Task<User> GetByExternalId(string externalId)
        {
            return _usersCollection.Find(x => x.ExternalId == externalId).FirstOrDefaultAsync();
        }
    }
}
