using System.ComponentModel.DataAnnotations;

namespace MiniJobBoard.Infrastructure.Entities;

public class JobApplication
{
    public int Id { get; set; }
    
    [Required]
    public string ApplicantId { get; set; } = string.Empty;
    
    [Required]
    public int JobId { get; set; }
    
    public string? CoverLetter { get; set; }
    
    public string? ResumeUrl { get; set; }
    
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ApplicationUser Applicant { get; set; } = null!;
    public Job Job { get; set; } = null!;
}

public enum ApplicationStatus
{
    Pending,
    Reviewed,
    Shortlisted,
    Rejected,
    Accepted
}
