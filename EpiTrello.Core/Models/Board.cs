namespace EpiTrello.Core.Models;

public class Board
{
    public long Id { get; set; }
    
    public int WorkspaceId { get; set; }
    
    public string Title { get; set; }
    
    public string Image { get; set; }
    
    public List<Stage> Stages { get; set; } = new();
    
    public List<Block> Blocks { get; set; } = new();
}
