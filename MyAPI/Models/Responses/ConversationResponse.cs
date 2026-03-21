public class ConversationResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Type { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
}