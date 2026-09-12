using IncidentDashboard.Api.Data;
using IncidentDashboard.Api.DTOs;
using IncidentDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentDashboard.Api.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return Unauthorized(new { message = "Credenciales inválidas" });
        return Ok(new { token = tokens.Create(user), user = new { user.Id, user.Name, user.Email, role = user.Role.ToString() } });
    }
}

