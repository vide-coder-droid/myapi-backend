public class SendMessageResponseDto
{
    public Guid ConversationId { get; set; }
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public Guid SenderId { get; set; }
    public DateTime CreatedAt { get; set; }
}