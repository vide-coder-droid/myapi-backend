public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; } 
    public string Content { get; set; } = "";
}