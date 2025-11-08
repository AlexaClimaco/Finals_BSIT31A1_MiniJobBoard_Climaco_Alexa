using System.ComponentModel.DataAnnotations;

namespace MiniJobBoard.Web.Models;

public class ApplyJobViewModel
{
    public int JobId { get; set; }
    
    public string JobTitle { get; set; } = string.Empty;

    [Display(Name = "Cover Letter")]
    [DataType(DataType.MultilineText)]
    public string? CoverLetter { get; set; }

    [Display(Name = "Resume URL")]
    [DataType(DataType.Url)]
    public string? ResumeUrl { get; set; }
}
