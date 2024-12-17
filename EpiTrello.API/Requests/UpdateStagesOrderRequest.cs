using EpiTrello.Core.Models;

namespace EpiTrello.API.Requests;

public class UpdateStagesOrderRequest
{
    public List<Stage> Stages { get; set; } = new();
}
