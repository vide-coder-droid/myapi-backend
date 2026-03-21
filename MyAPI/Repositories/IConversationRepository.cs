using MyAPI.Models.Entities;

public interface IConversationRepository
{
    Task<List<Conversation>> GetUserConversations(Guid userId);
    Task<List<Message>> GetMessages(Guid conversationId);
    Task<Message> AddMessage(Message message);

    Task<Conversation?> GetPrivateConversation(Guid userA, Guid userB);
    Task<Conversation> CreatePrivateConversation(Guid userA, Guid userB);
}