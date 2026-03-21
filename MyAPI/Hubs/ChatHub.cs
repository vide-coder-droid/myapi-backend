using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace MyAPI.Hubs
{
    public class ChatHub : Hub
    {
        // Thread-safe online users
        private static ConcurrentDictionary<string, HashSet<string>> OnlineUsers = new();

        // Khi user connect
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                if (!OnlineUsers.ContainsKey(userId))
                    OnlineUsers[userId] = new HashSet<string>();

                OnlineUsers[userId].Add(Context.ConnectionId);

                await Clients.All.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        // Khi user disconnect
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && OnlineUsers.ContainsKey(userId))
            {
                OnlineUsers[userId].Remove(Context.ConnectionId);

                if (OnlineUsers[userId].Count == 0)
                {
                    OnlineUsers.TryRemove(userId, out _);
                    await Clients.All.SendAsync("UserOffline", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        /* Broadcast chat
        public async Task SendMessage(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, message);
        }*/

            // Private chat
        public async Task SendPrivateMessage(string toUserId, string message)
        {
            var fromUserId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (OnlineUsers.TryGetValue(toUserId, out var connections))
            {
                foreach (var connId in connections)
                {
                    await Clients.Client(connId)
                        .SendAsync("ReceivePrivateMessage", fromUserId, message);
                }
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

        /* Send message to room
        public async Task SendRoomMessage(string roomId, string userId, string message)
        {
            await Clients.Group(roomId)
            .SendAsync("ReceiveRoomMessage", new
            {
                conversationId = roomId, 
                userId,
                content = message,
                createdAt = DateTime.UtcNow
            });
        }?*/
    }
}