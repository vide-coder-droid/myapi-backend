using MyAPI.Models.Entities;
using MyAPI.Models.Responses;
using MyAPI.Repositories;

namespace MyAPI.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatService(
            IConversationRepository repo,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
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

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("Không lấy được userId từ token");

            return Guid.Parse(userId);
        }

        public async Task<ApiResponse<SendMessageResponseDto>> SendMessage(Guid conversationId, string content)
        {
            var userId = GetCurrentUserId();

            if (conversationId == Guid.Empty)
                return ApiResponse<SendMessageResponseDto>.Fail("ConversationId không hợp lệ");

            if (string.IsNullOrWhiteSpace(content))
                return ApiResponse<SendMessageResponseDto>.Fail("Content rỗng");

            var conversation = await _repo.GetConversationWithMembers(conversationId);

            if (conversation == null)
                return ApiResponse<SendMessageResponseDto>.Fail("Conversation không tồn tại");

            if (!conversation.Members.Any(m => m.UserId == userId))
                return ApiResponse<SendMessageResponseDto>.Fail("Bạn không thuộc conversation này");

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = userId,
                Content = content,
                MessageType = "text",
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.AddMessage(message);

            var dto = new SendMessageResponseDto
            {
                ConversationId = conversation.Id,
                Id = saved.Id,
                Content = saved.Content,
                SenderId = userId,
                CreatedAt = saved.CreatedAt
            };

            return ApiResponse<SendMessageResponseDto>.Ok(dto, "Message sent");
        }
    }
}