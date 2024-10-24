using Microsoft.AspNetCore.Mvc;

namespace EpiTrello.API.Controllers;

public class BaseController : ControllerBase
{
    protected string ClientIpAddress { get; private set; }
    
    public BaseController()
    {
        ClientIpAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();

        if (HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
        {
            ClientIpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault();
        }
        
        if (string.IsNullOrEmpty(ClientIpAddress) || ClientIpAddress == "::1")
        {
            ClientIpAddress = "127.0.0.1";
        }
    }
}