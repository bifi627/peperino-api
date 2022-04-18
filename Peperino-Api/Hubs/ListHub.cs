using Microsoft.AspNetCore.SignalR;

namespace Peperino_Api.Hubs
{
    public class ListHub : Hub
    {
        public ListHub()
        {
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.Client(Context.ConnectionId).SendAsync("connected");
        }

        public Task JoinList(string listSlug)
        {
            var task = Groups.AddToGroupAsync(Context.ConnectionId, listSlug);
            Clients.Client(Context.ConnectionId).SendAsync("ListJoined");
            return task;
        }

        public Task LeaveList(string listSlug)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, listSlug);
        }
    }
}
