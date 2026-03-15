namespace MyAPI.Models.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public string Type { get; set; } = "private";

    public string? Title { get; set; }

    public Guid? LastMessageId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}