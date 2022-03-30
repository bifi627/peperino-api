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

        public async Task<bool> UpdateItem(User user, string slug, ListItem listItem)
        {
            var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Where(u => u.Content.Slug == slug && u.Content.ListItems.Any(i => i.Id == listItem.Id));
            var update = Builders<ShareableEntity<List>>.Update.Set(f => f.Content.ListItems[-1], listItem);
            var updateResult = await _listsCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<ListItem?> AddTextItem(User user, string slug, string item)
        {
            var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);
            ListItem newListItem = new() { Text = item, Type = ItemType.Text };
            var update = Builders<ShareableEntity<List>>.Update.Push(f => f.Content.ListItems, newListItem);

            var updateResult = await _listsCollection.UpdateOneAsync(filter, update);

            if (updateResult.IsAcknowledged)
            {
                return newListItem;
            }

            return null;
        }

        public async Task<bool> DeleteItem(User user, string slug, Guid id)
        {
            var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Where(u => u.Content.Slug == slug);
            var update = Builders<ShareableEntity<List>>.Update.PullFilter(f => f.Content.ListItems, listItem => listItem.Id == id);
            var updateResult = await _listsCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<List?> MoveItem(User user, string slug, int from, int to)
        {

            var list = await this.GetBySlug(user, slug);

            if (list is not null)
            {
                var array = list.ListItems.ToArray();
                ShiftElement(array, from, to);
                var modifiedList = array.ToList();

                var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);
                var update = Builders<ShareableEntity<List>>.Update.Set(f => f.Content.ListItems, modifiedList);
                var updateResult = await _listsCollection.UpdateOneAsync(filter, update);

                if (updateResult.IsAcknowledged)
                {
                    list.ListItems = modifiedList;
                    return list;
                }
            }


            return null;
        }

        private static void ShiftElement<T>(T[] array, int oldIndex, int newIndex)
        {
            // TODO: Argument validation
            if (oldIndex == newIndex)
            {
                return; // No-op
            }
            T tmp = array[oldIndex];
            if (newIndex < oldIndex)
            {
                // Need to move part of the array "up" to make room
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            }
            else
            {
                // Need to move part of the array "down" to fill the gap
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            }
            array[newIndex] = tmp;
        }
    }


}


