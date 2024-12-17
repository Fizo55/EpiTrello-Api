namespace EpiTrello.Core.Models;

public class InviteLink
{
    public int Id { get; set; }
    public long BoardId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = "Active";
}