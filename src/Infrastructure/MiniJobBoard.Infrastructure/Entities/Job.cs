using System.ComponentModel.DataAnnotations;

namespace MiniJobBoard.Infrastructure.Entities;

public class Job
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Location { get; set; }
    
    [StringLength(50)]
    public string? JobType { get; set; } // Full-time, Part-time, Contract, etc.
    
    public decimal? Salary { get; set; }
    
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? Deadline { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Foreign key
    [Required]
    public string EmployerId { get; set; } = string.Empty;
    
    // Navigation properties
    public ApplicationUser Employer { get; set; } = null!;
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
