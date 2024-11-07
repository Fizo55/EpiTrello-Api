using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EpiTrello.API.Interfaces;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IRequestTrackingService _requestTrackingService;

    public AuthController(UserService userService, IConfiguration configuration, IRequestTrackingService requestTrackingService)
    {
        _userService = userService;
        _configuration = configuration;
        _requestTrackingService = requestTrackingService;
    }
    
    // POST: /auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        bool isAvailable = await _userService.IsUsernameAvailableAsync(user.Username);

        if (!isAvailable)
        {
            return BadRequest($"This username is already taken.");
        }

        user.Password = HashPassword(user.Password);
        await _userService.CreateUserAsync(user);
        return Ok();
    }
    
    // POST: /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        if (_requestTrackingService.IsBanned(clientIp, out DateTime bannedUntil))
        {
            return BadRequest($"Too many requests. You are banned until {bannedUntil}.");
        }

        if (!_requestTrackingService.CheckRequestLimit(clientIp))
        {
            _requestTrackingService.BanClient(clientIp);
            return BadRequest("Too many requests. You have been temporarily banned for 5 minutes.");
        }
        
        User? foundUser = await _userService.GetUserAsync(user.Username, HashPassword(user.Password));

        if (foundUser == null)
        {
            return BadRequest("Incorrect credentials");
        }

        string token = GenerateJwtToken(foundUser);
        return Ok(token);
    }
    
    private string HashPassword(string password)
    {
        using SHA512 sha512 = SHA512.Create();
        var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["TokenLifetimeMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}