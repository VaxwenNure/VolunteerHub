using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Models;

namespace VolunteerHub.Data.Data;

public class VolunteerHubContext : DbContext
{
    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<HelpType> HelpTypes => Set<HelpType>();
    public DbSet<VolunteerHelp> VolunteerHelps => Set<VolunteerHelp>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // SQLite local file near the executable
        optionsBuilder.UseSqlite("Data Source=volunteerhub.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Volunteer>()
            .Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Volunteer>()
            .Property(x => x.Phone)
            .HasMaxLength(30);

        modelBuilder.Entity<HelpType>()
            .Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<VolunteerHelp>()
            .Property(x => x.Description)
            .HasMaxLength(300);

        modelBuilder.Entity<VolunteerHelp>()
            .HasOne(x => x.Volunteer)
            .WithMany(v => v.VolunteerHelps)
            .HasForeignKey(x => x.VolunteerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VolunteerHelp>()
            .HasOne(x => x.HelpType)
            .WithMany(t => t.VolunteerHelps)
            .HasForeignKey(x => x.HelpTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
