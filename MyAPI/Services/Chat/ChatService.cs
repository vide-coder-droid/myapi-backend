using MyAPI.Models.Entities;
using MyAPI.Models.Responses;
using MyAPI.Repositories;

namespace MyAPI.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _repo;

        public ChatService(IConversationRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<object>> GetUserConversations(Guid userId)
        {
            var conversations = await _repo.GetUserConversations(userId);

            if (conversations == null || !conversations.Any())
            {
                return ApiResponse<object>.Ok(new { message = "No conversations" });
            }

            var result = conversations.Select(c =>
            {
                string title = c.Title ?? "No name";

                if (c.Type == "private")
                {
                    var otherUser = c.Members
                        .FirstOrDefault(m => m.UserId != userId)?.User;

                    if (otherUser != null)
                    {
                        title = otherUser.Profile?.FullName ?? otherUser.Username;
                    }
                }

                return new
                {
                    c.Id,
                    Title = title,
                    c.Type,
                    c.CreatedAt,
                    LastMessage = c.Messages?.FirstOrDefault()?.Content,
                    LastMessageTime = c.Messages?.FirstOrDefault()?.CreatedAt
                };
            });

            return ApiResponse<object>.Ok(result, "Conversations retrieved");
        }

        public async Task<ApiResponse<object>> GetMessages(Guid conversationId)
        {
            var messages = await _repo.GetMessages(conversationId);

            var result = messages.Select(m => new
            {
                m.Id,
                m.SenderId,
                m.Content,
                m.CreatedAt
            });

            return ApiResponse<object>.Ok(result, "Messages retrieved");
        }

        public async Task<ApiResponse<object>> SendMessage(Guid senderId, Guid conversationId, string content)
        {
            if (conversationId == Guid.Empty)
                return ApiResponse<object>.Fail("ConversationId không hợp lệ");

            if (string.IsNullOrWhiteSpace(content))
                return ApiResponse<object>.Fail("Content rỗng");

            // 1. lấy conversation + members
            var conversation = await _repo.GetConversationWithMembers(conversationId);

            if (conversation == null)
                return ApiResponse<object>.Fail("Conversation không tồn tại");

            // 2. check sender có trong room không
            if (!conversation.Members.Any(m => m.UserId == senderId))
                return ApiResponse<object>.Fail("Bạn không thuộc conversation này");

            // 3. lưu message
            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = senderId,
                Content = content,
                MessageType = "text",
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.AddMessage(message);

            return ApiResponse<object>.Ok(new
            {
                conversationId = conversation.Id,
                id = saved.Id,
                content = saved.Content,
                createdAt = saved.CreatedAt
            }, "Message sent");
        }
    }
}