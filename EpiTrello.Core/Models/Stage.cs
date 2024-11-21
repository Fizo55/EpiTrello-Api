namespace EpiTrello.Core.Models;

public class Stage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long BoardId { get; set; }
    public Board Board { get; set; }
}
