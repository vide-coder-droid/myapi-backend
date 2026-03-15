using Microsoft.AspNetCore.SignalR;

namespace MyAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, message);
        }
    }
}