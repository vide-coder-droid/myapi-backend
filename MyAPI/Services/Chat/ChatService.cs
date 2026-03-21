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
                return ApiResponse<object>.Ok(null, "No conversations");
            }

            var result = conversations.Select(c => new
            {
                c.Id,
                c.Title,
                c.Type,
                c.CreatedAt,
                LastMessage = c.Messages?.FirstOrDefault()?.Content,
                LastMessageTime = c.Messages?.FirstOrDefault()?.CreatedAt
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

        public async Task<ApiResponse<object>> SendMessage(Guid senderId, Guid receiverId, string content)
        {
            if (receiverId == Guid.Empty)
                return ApiResponse<object>.Fail("ReceiverId không hợp lệ");

            if (string.IsNullOrWhiteSpace(content))
                return ApiResponse<object>.Fail("Content rỗng");

            // 1. tìm conversation
            var conversation = await _repo.GetPrivateConversation(senderId, receiverId);

            // 2. nếu chưa có → tạo mới
            if (conversation == null)
            {
                conversation = await _repo.CreatePrivateConversation(senderId, receiverId);
            }

            // ❗ safety check
            if (conversation == null)
                return ApiResponse<object>.Fail("Không tạo được conversation");

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