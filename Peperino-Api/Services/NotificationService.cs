using Microsoft.AspNetCore.SignalR;
using Peperino_Api.Hubs;
using Peperino_Api.Models.User;

namespace Peperino_Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ListHub> notificationHub;

        public NotificationService(IHubContext<ListHub> notificationHub)
        {
            this.notificationHub = notificationHub;
        }

        public Task SendListUpdatedNotification(string slug, string userId)
        {
            return this.notificationHub.Clients.Group(slug).SendAsync("Update", userId);
        }
    }
}
