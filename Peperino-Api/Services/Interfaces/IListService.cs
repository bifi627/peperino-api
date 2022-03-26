using MongoDB.Bson;
using Peperino_Api.Models;
using Peperino_Api.Models.List;
using Peperino_Api.Models.User;
using System.Collections.Generic;

namespace Peperino_Api.Services
{
    public interface IListService
    {
        public Task<List?> GetById(User user, ObjectId id);
        public Task<string> Create(User user, List item);
        public Task<IEnumerable<List>> GetAllForUser(User user);
        public Task<List?> GetBySlug(User user, string slug);
        public Task<bool> CheckSlugAvailable(string slug);
        public Task<ListItem?> AddTextItem(User user, string slug, string item);
        public Task<bool> CheckItem(User user, string slug, Guid itemId, bool @checked);
        public Task<bool> UpdateItem(User user, string slug, Guid id, string item);
        public Task<bool> DeleteItem(User user, string slug, Guid id);
    }
}
