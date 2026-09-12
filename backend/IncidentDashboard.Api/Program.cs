using System.Text;
using System.Text.Json.Serialization;
using IncidentDashboard.Api.Data;
using IncidentDashboard.Api.Models;
using IncidentDashboard.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<TokenService>(); builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new() { ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true, ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key) });
builder.Services.AddAuthorization(); builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build(); app.UseSwagger(); app.UseSwaggerUI(); app.UseCors(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        var admin = new User { Name = "Ana Administradora", Email = "admin@incidentes.local", PasswordHash = BCrypt.Net.BCrypt.HashPassword(builder.Configuration["Seed:AdminPassword"] ?? throw new InvalidOperationException("Falta Seed__AdminPassword")), Role = UserRole.Admin };
        var tech = new User { Name = "Carlos Técnico", Email = "carlos@incidentes.local", PasswordHash = BCrypt.Net.BCrypt.HashPassword(builder.Configuration["Seed:TechnicianPassword"] ?? throw new InvalidOperationException("Falta Seed__TechnicianPassword")), Role = UserRole.Technician };
        var requester = new User { Name = "María López", Email = "maria@empresa.local", PasswordHash = BCrypt.Net.BCrypt.HashPassword(builder.Configuration["Seed:RequesterPassword"] ?? throw new InvalidOperationException("Falta Seed__RequesterPassword")), Role = UserRole.Requester };
        db.Users.AddRange(admin, tech, requester);
        db.SaveChanges();
        db.Incidents.AddRange(
            new Incident { Title = "VPN sin conexión", Description = "No es posible conectar desde casa.", Priority = Priority.High, RequesterId = requester.Id, TechnicianId = tech.Id, DueAt = DateTime.UtcNow.AddHours(4) },
            new Incident { Title = "Acceso a carpeta compartida", Description = "Permisos pendientes.", Priority = Priority.Medium, Status = IncidentStatus.Paused, RequesterId = requester.Id, DueAt = DateTime.UtcNow.AddDays(1) },
            new Incident { Title = "Monitor externo", Description = "Pantalla sin señal.", Priority = Priority.Low, Status = IncidentStatus.Resolved, RequesterId = requester.Id, TechnicianId = tech.Id, ResolvedAt = DateTime.UtcNow.AddHours(-2) });
        db.SaveChanges();
    }
}
app.Run();
