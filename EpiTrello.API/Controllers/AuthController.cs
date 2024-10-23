using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EpiTrello.Core.Models;
using EpiTrello.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EpiTrello.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }
    
    // POST: /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
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