using FirebaseAdmin;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Peperino_Api.Models.User;
using Peperino_Api.Startup;

namespace Peperino_Api.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly FirebaseApp firebase;

        public UserService(IOptions<MongoSettings> mongoSettings, FirebaseApp firebase)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(mongoSettings.Value.UsersCollectionName);
            this.firebase = firebase;
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
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.ExternalId, externalId);
            return _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<User> GetByUsername(string username)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Username, username);
            return _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> Exists(User user)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Username, user.Username) | Builders<User>.Filter.Eq(u => u.ExternalId, user.ExternalId);
            var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            return result is not null;
        }

        public async Task<bool> CheckUsername(string username)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            return result is null;
        }

        public async Task<User> CreateNewUser(User user, string email, string password)
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.GetAuth(firebase);

            // Create firebase user
            var newFirebaseUser = await auth.CreateUserAsync(new FirebaseAdmin.Auth.UserRecordArgs()
            {
                Email = email,
                Password = password,
                DisplayName = user.Username,
            });

            user.ExternalId = newFirebaseUser.Uid;

            // Check if a user with this username or external id already exists
            if (await this.CheckUsername(user.Username))
            {
                await this.CreateAsync(user);
            }
            else
            {
                // Delete firebase user if peperino user cant be created
                await auth.DeleteUserAsync(user.ExternalId);
                throw new Exception("Benutzername ist nicht verfügbar.");
            }

            return user;
        }

        public async Task<User> HandleProviderLogin(User user)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.ExternalId, user.ExternalId);
            var result = await _usersCollection.Find(filter).FirstOrDefaultAsync();

            if(result is null)
            {
                await this.CreateAsync(user);
            }

            return user;
        }
    }
}
