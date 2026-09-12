namespace IncidentDashboard.Api.Models;

public enum UserRole { Admin, Technician, Requester }
public enum Priority { Low, Medium, High, Critical }
public enum IncidentStatus { Pending, Paused, Resolved }

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public class Incident
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public IncidentStatus Status { get; set; } = IncidentStatus.Pending;
    public int RequesterId { get; set; }
    public User? Requester { get; set; }
    public int? TechnicianId { get; set; }
    public User? Technician { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

