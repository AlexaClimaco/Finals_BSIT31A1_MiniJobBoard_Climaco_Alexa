namespace MiniJobBoard.Application.Jobs.Dtos;

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? JobType { get; set; }
    public decimal? Salary { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; }
    public string EmployerId { get; set; } = string.Empty;
    public string? EmployerName { get; set; }
}

public class CreateJobDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? JobType { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? Deadline { get; set; }
    public string EmployerId { get; set; } = string.Empty;
}

public class UpdateJobDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? JobType { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; }
}

