using MyAPI.Models.Responses;

namespace MyAPI.Services.Chat
{
    public interface IChatService
    {
        Task<ApiResponse<object>> GetUserConversations(Guid userId);
        Task<ApiResponse<object>> GetMessages(Guid conversationId);
        Task<ApiResponse<object>> SendMessage(Guid conversationId, Guid senderId, string content);
    }
}