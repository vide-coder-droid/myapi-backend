using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models.Entities;

public class DbConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public DbConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Conversation>> GetUserConversations(Guid userId)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
                    .ThenInclude(u => u.Profile)
            .Where(c => c.Members.Any(m => m.UserId == userId))
            .OrderByDescending(c => c.Messages.Any()
                ? c.Messages.Max(m => m.CreatedAt)
                : c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Message>> GetMessages(Guid conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId && m.DeletedAt == null)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Message> AddMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<Conversation?> GetPrivateConversation(Guid userA, Guid userB)
    {
        return await _context.Conversations
            .Include(c => c.Members)
            .Where(c => c.Type == "private")
            .Where(c => c.Members.Any(m => m.UserId == userA) &&
                        c.Members.Any(m => m.UserId == userB))
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation> CreatePrivateConversation(Guid userA, Guid userB)
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Type = "private",
            CreatedAt = DateTime.UtcNow,
            Members = new List<ConversationMember>
        {
            new() { UserId = userA },
            new() { UserId = userB }
        }
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task<Conversation?> GetConversationWithMembers(Guid conversationId)
    {
        return await _context.Conversations
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }
}