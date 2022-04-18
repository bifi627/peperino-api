using Peperino_Api.Models.User;

namespace Peperino_Api.Services
{
    public interface INotificationService
    {
        public Task SendListUpdatedNotification(string slug, string userId);
    }
}
