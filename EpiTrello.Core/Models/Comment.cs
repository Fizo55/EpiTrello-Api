namespace EpiTrello.Core.Models;

public class Comment
{
    public long Id { get; set; }
    public long BlockId { get; set; }
    public long UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Block? Block { get; set; }
    public User? User { get; set; }
}