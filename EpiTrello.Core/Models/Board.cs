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

public class Stage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long BoardId { get; set; }
    public Board Board { get; set; }
}

public class Block
{
    public long Id { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public long BoardId { get; set; }
    public Board Board { get; set; }
}
