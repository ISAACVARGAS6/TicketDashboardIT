using IncidentDashboard.Api.Models;

namespace IncidentDashboard.Api.DTOs;
public record LoginRequest(string Email, string Password);
public record CreateIncidentRequest(string Title, string Description, Priority Priority, int RequesterId, int? TechnicianId, DateTime? DueAt);
public record UpdateIncidentRequest(string? Title, string? Description, Priority? Priority, IncidentStatus? Status, int? TechnicianId, DateTime? DueAt);

