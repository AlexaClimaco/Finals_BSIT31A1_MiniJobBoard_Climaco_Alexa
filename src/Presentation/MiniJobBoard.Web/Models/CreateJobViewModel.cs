using System.ComponentModel.DataAnnotations;

namespace MiniJobBoard.Web.Models;

public class CreateJobViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Job Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job Description")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Location { get; set; }

    [Display(Name = "Job Type")]
    public string? JobType { get; set; }

    [Display(Name = "Salary")]
    [DataType(DataType.Currency)]
    public decimal? Salary { get; set; }

    [Display(Name = "Application Deadline")]
    [DataType(DataType.Date)]
    public DateTime? Deadline { get; set; }
}
