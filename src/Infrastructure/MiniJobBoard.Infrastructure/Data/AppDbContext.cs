using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniJobBoard.Infrastructure.Entities;

namespace MiniJobBoard.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Job entity
        builder.Entity<Job>(entity =>
        {
            entity.HasKey(j => j.Id);
            
            entity.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(j => j.Description)
                .IsRequired();
            
            entity.Property(j => j.Salary)
                .HasColumnType("decimal(18,2)");
            
            // Relationship: Job -> Employer (ApplicationUser)
            entity.HasOne(j => j.Employer)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Relationship: Job -> Applications
            entity.HasMany(j => j.Applications)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure JobApplication entity
        builder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            // Relationship: JobApplication -> Applicant (ApplicationUser)
            entity.HasOne(a => a.Applicant)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Composite unique index to prevent duplicate applications
            entity.HasIndex(a => new { a.ApplicantId, a.JobId })
                .IsUnique();
        });

        // Configure ApplicationUser entity
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName)
                .HasMaxLength(100);
            
            entity.Property(u => u.LastName)
                .HasMaxLength(100);
        });
    }
}

