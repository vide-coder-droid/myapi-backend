public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}