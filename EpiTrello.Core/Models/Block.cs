namespace EpiTrello.Core.Models;

public class Block
{
    public long Id { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public long BoardId { get; set; }
    public Board Board { get; set; }
}