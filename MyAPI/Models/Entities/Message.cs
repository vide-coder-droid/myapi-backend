namespace MyAPI.Models.Entities;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string Content { get; set; } = "";

    public string MessageType { get; set; } = "text";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? EditedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Conversation Conversation { get; set; } = null!;

    public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();
}