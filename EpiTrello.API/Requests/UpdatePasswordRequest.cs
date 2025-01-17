namespace EpiTrello.API.Requests;

public class UpdatePasswordRequest
{
    public string CurrentPassword { get; set; }
    
    public string NewPassword { get; set; }
}