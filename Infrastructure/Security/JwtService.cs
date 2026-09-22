using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoleManagementBackend.Application.Interfaces;

namespace RoleManagementBackend.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int userId, string email, string roleName, IEnumerable<string> permissions)
    {
        Console.WriteLine("\n................................................................................");
        Console.WriteLine(">>> [STEP 4: SECURITY - GENERATING JWT TOKEN]");
        Console.WriteLine("    [FILE]     : Infrastructure/Security/JwtService.cs");
        Console.WriteLine("    [METHOD]   : GenerateToken(int userId, string email, string roleName, ...)");
        Console.WriteLine($"   [CLAIMS]   : Sub={userId}, Email='{email}', Role='{roleName}'");

        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresInMinutes = double.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var exp) ? exp : 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Role, roleName)
        };

        int permCount = 0;
        foreach (var permission in permissions)
        {
            if (!string.IsNullOrWhiteSpace(permission))
            {
                claims.Add(new Claim("permission", permission));
                permCount++;
            }
        }
        Console.WriteLine($"    [ACTION]   : Added {permCount} permission claims to token. Signing with HmacSha256 (Expires in {expiresInMinutes} mins)...");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"   [TOKEN]    : Token generated successfully! (First 20 chars: {tokenString[..Math.Min(20, tokenString.Length)]}...)");
        Console.WriteLine("    [RETURN]   : Returning token string back to AuthService.cs");
        Console.WriteLine("................................................................................");

        return tokenString;
    }
}
