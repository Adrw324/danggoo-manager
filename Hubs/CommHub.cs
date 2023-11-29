using Microsoft.AspNetCore.SignalR;

namespace danggoo_manager.Hubs
{
    public class CommHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}