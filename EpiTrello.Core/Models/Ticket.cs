using System.Text.Json.Serialization;

namespace EpiTrello.Core.Models;

public class Ticket
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }

    public long BlockId { get; set; }
    
    [JsonIgnore]
    public Block? Block { get; set; }
}