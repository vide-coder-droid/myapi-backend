namespace MyAPI.Models.Entities;

public class Attachment
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public string FileUrl { get; set; } = "";

    public string FileType { get; set; } = "file";

    public long FileSize { get; set; }

    public Message Message { get; set; } = null!;
}