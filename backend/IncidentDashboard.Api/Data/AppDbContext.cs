using IncidentDashboard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentDashboard.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Incident>().HasOne(x => x.Requester).WithMany().HasForeignKey(x => x.RequesterId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Incident>().HasOne(x => x.Technician).WithMany().HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.SetNull);
    }
}

