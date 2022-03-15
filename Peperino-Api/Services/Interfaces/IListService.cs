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
    }
}
