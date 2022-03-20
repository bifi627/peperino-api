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
        private readonly IMongoCollection<ShareableEntity<List>> _itemsCollection;

        public ListService(IOptions<MongoSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _itemsCollection = mongoDatabase.GetCollection<ShareableEntity<List>>(mongoSettings.Value.ItemsCollectionName);
        }

        public async Task<List?> GetById(User user, ObjectId id)
        {
            var item = await _itemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

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

            await _itemsCollection.InsertOneAsync(ownableItem);

            return ownableItem.Id.ToString();
        }
    }
}
