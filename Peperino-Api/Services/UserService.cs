using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Peperino_Api.Models;

namespace Peperino_Api.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IOptions<MongoSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(mongoSettings.Value.UsersCollectionName);
        }

        public IEnumerable<User> Get()
        {
            return _usersCollection.Find(_ => true).ToEnumerable();
        }

        public async Task<User?> GetAsync(string id)
        {
            return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(User newBook)
        {
            await _usersCollection.InsertOneAsync(newBook);
        }

        public async Task UpdateAsync(string id, User updatedBook)
        {
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);
        }

        public async Task RemoveAsync(string id)
        {
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
