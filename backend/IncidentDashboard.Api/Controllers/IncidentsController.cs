using IncidentDashboard.Api.Data;
using IncidentDashboard.Api.DTOs;
using IncidentDashboard.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentDashboard.Api.Controllers;
[ApiController, Route("api/incidents"), Authorize]
public class IncidentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] IncidentStatus? status, [FromQuery] Priority? priority)
    {
        var query = db.Incidents.Include(x => x.Requester).Include(x => x.Technician).AsQueryable();
        if (status is not null) query = query.Where(x => x.Status == status);
        if (priority is not null) query = query.Where(x => x.Priority == priority);
        var items = await query.OrderByDescending(x => x.CreatedAt).Select(x => new { x.Id, x.Title, x.Description, priority = x.Priority.ToString(), status = x.Status == IncidentStatus.Pending && x.DueAt < DateTime.UtcNow ? "Overdue" : x.Status.ToString(), requester = x.Requester!.Name, technician = x.Technician == null ? null : x.Technician.Name, x.CreatedAt, x.DueAt, x.ResolvedAt }).ToListAsync();
        return Ok(items);
    }

    [HttpPost, Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Create(CreateIncidentRequest request)
    {
        var item = new Incident { Title = request.Title, Description = request.Description, Priority = request.Priority, RequesterId = request.RequesterId, TechnicianId = request.TechnicianId, DueAt = request.DueAt };
        db.Incidents.Add(item); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPatch("{id}"), Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Update(int id, UpdateIncidentRequest request)
    {
        var item = await db.Incidents.FindAsync(id); if (item is null) return NotFound();
        if (request.Title is not null) item.Title = request.Title;
        if (request.Description is not null) item.Description = request.Description;
        if (request.Priority is not null) item.Priority = request.Priority.Value;
        if (request.Status is not null) { item.Status = request.Status.Value; item.ResolvedAt = item.Status == IncidentStatus.Resolved ? DateTime.UtcNow : null; }
        if (request.TechnicianId is not null) item.TechnicianId = request.TechnicianId;
        if (request.DueAt is not null) item.DueAt = request.DueAt;
        await db.SaveChangesAsync(); return NoContent();
    }
}

