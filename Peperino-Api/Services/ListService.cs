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

        private FilterDefinition<ShareableEntity<List>> GetSecurityFilter(User user)
        {
            return Builders<ShareableEntity<List>>.Filter.Eq(item => item.OwnerId, user.Id) | Builders<ShareableEntity<List>>.Filter.AnyEq(item => item.SharedWith, user.Id);
        }

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
            var filter = GetSecurityFilter(user);
            var lists = await _listsCollection.FindAsync(filter);
            return lists.ToEnumerable().Select(s => s.Content);
        }

        public async Task<bool> CheckSlugAvailable(string slug)
        {
            FilterDefinition<ShareableEntity<List>> filter = Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);
            var result = await _listsCollection.Find(filter).FirstOrDefaultAsync();
            return result is null;
        }

        public async Task<List?> GetBySlug(User user, string slug)
        {
            var securityFilter = GetSecurityFilter(user);
            var filter = securityFilter & Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);
            var result = await _listsCollection.Find(filter).FirstOrDefaultAsync();
            return result?.Content;
        }

        public async Task<bool> CheckItem(User user, string slug, Guid itemId, bool @checked)
        {
            var list = await this.GetBySlug(user, slug);

            if (list is not null)
            {
                var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Where(u => u.Content.Slug == slug && u.Content.ListItems.Any(i => i.Id == itemId));
                var update = Builders<ShareableEntity<List>>.Update.Set(f => f.Content.ListItems[-1].Checked, @checked);
                var updateResult = await _listsCollection.UpdateOneAsync(filter, update);
                return updateResult.IsAcknowledged;
            }

            return false;
        }

        public async Task<ListItem?> AddTextItem(User user, string slug, string item)
        {
            var list = await this.GetBySlug(user, slug);

            if (list is not null)
            {
                var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);

                ListItem newListItem = new ListItem() { Text = item };
                list.ListItems.Add(newListItem);

                var update = Builders<ShareableEntity<List>>.Update.Set(f => f.Content.ListItems, list.ListItems);

                var updateResult = await _listsCollection.UpdateOneAsync(filter, update);
                return newListItem;
            }

            return null;
        }

        public Task<bool> UpdateItem(User user, string slug, Guid id, string item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItem(User user, string slug, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
