public class RoomMessageDto
{
    public Guid ConversationId { get; set; }
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}