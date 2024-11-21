using System.Text.Json.Serialization;

namespace EpiTrello.Core.Models;

public class Stage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long BoardId { get; set; }
    
    [JsonIgnore]
    public Board? Board { get; set; }
}
