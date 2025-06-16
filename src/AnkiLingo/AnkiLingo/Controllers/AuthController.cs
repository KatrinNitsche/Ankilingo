using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GetToken([FromBody] LoginModel login)
    {
        if (login is null)
            return BadRequest();

        // Read expected credentials from configuration
        var expectedUsername = _config["Auth:Username"];
        var expectedPassword = _config["Auth:Password"];

        if (string.IsNullOrEmpty(expectedUsername) || string.IsNullOrEmpty(expectedPassword))
            return StatusCode(500, "Auth credentials are not configured.");

        if (login.Username != expectedUsername || login.Password != expectedPassword)
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username)
        };

        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer))
            return StatusCode(500, "JWT configuration is missing.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }


}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
