using System.Text.Json.Serialization;

namespace EpiTrello.Core.Models;

public class Ticket
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }

    public long BoardId { get; set; }

    [JsonIgnore]
    public Board? Board { get; set; }
}