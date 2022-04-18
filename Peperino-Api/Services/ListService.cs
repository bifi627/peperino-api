using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Peperino_Api.Hubs;
using Peperino_Api.Models.Entity;
using Peperino_Api.Models.List;
using Peperino_Api.Models.User;
using Peperino_Api.Startup;

namespace Peperino_Api.Services
{
    public class ListService : IListService
    {
        private readonly IMongoCollection<ShareableEntity<List>> _listsCollection;
        private readonly INotificationService notificationService;

        private FilterDefinition<ShareableEntity<List>> GetSecurityFilter(User user)
        {
            return Builders<ShareableEntity<List>>.Filter.Eq(item => item.OwnerId, user.Id) | Builders<ShareableEntity<List>>.Filter.AnyEq(item => item.SharedWith, user.Id);
        }

        public ListService(IOptions<MongoSettings> mongoSettings, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _listsCollection = mongoDatabase.GetCollection<ShareableEntity<List>>(mongoSettings.Value.ItemsCollectionName);
            this.notificationService = notificationService;
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

            var list = await this.GetBySlug(user, slug);

            await SendUpdateSignal(slug, user);

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
            await SendUpdateSignal(slug, user);
                return newListItem;
            }

            return null;
        }

        public async Task<bool> DeleteItem(User user, string slug, Guid id)
        {
            var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Where(u => u.Content.Slug == slug);
            var update = Builders<ShareableEntity<List>>.Update.PullFilter(f => f.Content.ListItems, listItem => listItem.Id == id);
            var updateResult = await _listsCollection.UpdateOneAsync(filter, update);

            await SendUpdateSignal(slug, user);

            return updateResult.IsAcknowledged;
        }

        public async Task<List?> MoveItem(User user, string slug, int from, int to)
        {
            var list = await this.GetBySlug(user, slug);

            if (list is not null)
            {
                var array = list.ListItems.Where(item => !item.Checked).ToArray();
                
                Extensions.ShiftElement(array, from, to);

                var modifiedList = array.ToList();
                modifiedList.AddRange(list.ListItems.Where(item => item.Checked));

                var filter = GetSecurityFilter(user) & Builders<ShareableEntity<List>>.Filter.Eq(u => u.Content.Slug, slug);
                var update = Builders<ShareableEntity<List>>.Update.Set(f => f.Content.ListItems, modifiedList);
                var updateResult = await _listsCollection.UpdateOneAsync(filter, update);

                if (updateResult.IsAcknowledged)
                {
                    await SendUpdateSignal(slug, user);

                    list.ListItems = modifiedList;
                    return list;
                }
            }

            return null;
        }

        private Task SendUpdateSignal(string slug, User user)
        {
            return notificationService.SendListUpdatedNotification(slug, user.ExternalId);
        }
    }
}


