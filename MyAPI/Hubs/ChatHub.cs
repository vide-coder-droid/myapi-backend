using Microsoft.AspNetCore.SignalR;

namespace MyAPI.Hubs
{
    public class ChatHub : Hub
    {
        // Lưu user online
        private static Dictionary<string, string> OnlineUsers = new();

        // Khi user connect
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                OnlineUsers[userId] = Context.ConnectionId;
            }

            await Clients.All.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }

        // Khi user disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = OnlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);

            if (!string.IsNullOrEmpty(user.Key))
            {
                OnlineUsers.Remove(user.Key);
                await Clients.All.SendAsync("UserOffline", user.Key);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Broadcast chat
        public async Task SendMessage(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, message);
        }

        // Private chat
        public async Task SendPrivateMessage(string toUserId, string message)
        {
            if (OnlineUsers.TryGetValue(toUserId, out var connectionId))
            {
                await Clients.Client(connectionId)
                    .SendAsync("ReceivePrivateMessage", Context.ConnectionId, message);
            }
        }

        // Join room
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        // Leave room
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }

        // Send message to room
        public async Task SendRoomMessage(string roomId, string userId, string message)
        {
            await Clients.Group(roomId)
                .SendAsync("ReceiveRoomMessage", userId, message);
        }
    }
}