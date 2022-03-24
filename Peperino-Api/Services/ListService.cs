using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Peperino_Api.Models.Entity;
using Peperino_Api.Models.List;
using Peperino_Api.Models.User;
using Peperino_Api.Startup;

namespace Peperino_Api.Services
{
    public class ListService : IListService
    {
        private readonly IMongoCollection<ShareableEntity<List>> _listsCollection;

        public ListService(IOptions<MongoSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _listsCollection = mongoDatabase.GetCollection<ShareableEntity<List>>(mongoSettings.Value.ItemsCollectionName);
        }

        public async Task<List?> GetById(User user, ObjectId id)
        {
            var item = await _listsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (item is not null && (item.OwnerId == user.Id || item.SharedWith.Contains(user.Id)))
            {
                return item.Content;
            }

            return null;
        }

        public async Task<string> Create(User user, List item)
        {
            var ownableItem = new ShareableEntity<List>
            {
                Content = item,
                OwnerId = user.Id,
            };

            await _listsCollection.InsertOneAsync(ownableItem);

            return ownableItem.Id.ToString();
        }

        public async Task<IEnumerable<List>> GetAllForUser(User user)
        {
            FilterDefinition<ShareableEntity<List>> filter = Builders<ShareableEntity<List>>.Filter.Eq(item => item.OwnerId, user.Id) | Builders<ShareableEntity<List>>.Filter.AnyEq(item => item.SharedWith, user.Id);
            var lists = await _listsCollection.FindAsync(filter);
            return lists.ToEnumerable().Select(s => s.Content);
        }
    }
}
