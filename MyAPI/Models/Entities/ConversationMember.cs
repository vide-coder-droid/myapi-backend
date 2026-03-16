namespace MyAPI.Models.Entities;

public class ConversationMember
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = "member";

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Conversation Conversation { get; set; } = null!;

    public User User { get; set; } = null!;
}