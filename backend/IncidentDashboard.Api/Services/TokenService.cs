using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IncidentDashboard.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace IncidentDashboard.Api.Services;
public class TokenService(IConfiguration configuration)
{
    public string Create(User user)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, user.Role.ToString()) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)));
    }
}

