using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : BaseController
{
    private readonly WebSocketManager _webSocketManager;
    private readonly IConfiguration _configuration;

    public EventController(WebSocketManager webSocketManager, IConfiguration configuration)
    {
        _webSocketManager = webSocketManager;
        _configuration = configuration;
    }
    
    [HttpGet("{boardId}/{token}/ws")]
    public async Task GetWebSocket(long boardId, string token)
    {
        ClaimsPrincipal? jwtToken = ValidateToken(token);

        if (jwtToken == null)
        {
            return;
        }
        
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketManager.Subscribe(boardId, webSocket);

            await ListenToClient(webSocket, boardId);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    private async Task ListenToClient(WebSocket webSocket, long boardId)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    _webSocketManager.Unsubscribe(boardId, webSocket);
                }
            }
        }
        catch
        {
            _webSocketManager.Unsubscribe(boardId, webSocket);
        }
    }
    
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}