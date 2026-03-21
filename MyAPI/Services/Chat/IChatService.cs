using MyAPI.Models.Responses;

namespace MyAPI.Services.Chat
{
    public interface IChatService
    {
        Task<ApiResponse<object>> GetUserConversations(Guid userId);
        Task<ApiResponse<object>> GetMessages(Guid conversationId);
        Task<ApiResponse<SendMessageResponseDto>> SendMessage(Guid conversationId, string content);
    }
}