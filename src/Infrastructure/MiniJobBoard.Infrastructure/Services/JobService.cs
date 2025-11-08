using Microsoft.EntityFrameworkCore;
using MiniJobBoard.Application.Jobs;
using MiniJobBoard.Application.Jobs.Dtos;
using MiniJobBoard.Infrastructure.Data;
using MiniJobBoard.Infrastructure.Entities;

namespace MiniJobBoard.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;

    public JobService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
    {
        var jobs = await _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();
        
        return jobs.Select(MapToDto);
    }

    public async Task<IEnumerable<JobDto>> GetActiveJobsAsync()
    {
        var jobs = await _context.Jobs
            .Include(j => j.Employer)
            .Where(j => j.IsActive && (j.Deadline == null || j.Deadline > DateTime.UtcNow))
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();
        
        return jobs.Select(MapToDto);
    }

    public async Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId)
    {
        var jobs = await _context.Jobs
            .Include(j => j.Applications)
            .Include(j => j.Employer)
            .Where(j => j.EmployerId == employerId)
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();
        
        return jobs.Select(MapToDto);
    }

    public async Task<JobDto?> GetJobByIdAsync(int jobId)
    {
        var job = await _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(j => j.Id == jobId);
        
        return job == null ? null : MapToDto(job);
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto dto)
    {
        var job = new Job
        {
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            JobType = dto.JobType,
            Salary = dto.Salary,
            Deadline = dto.Deadline,
            EmployerId = dto.EmployerId,
            PostedDate = DateTime.UtcNow,
            IsActive = true
        };
        
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        
        await _context.Entry(job).Reference(j => j.Employer).LoadAsync();
        return MapToDto(job);
    }

    public async Task<JobDto?> UpdateJobAsync(int jobId, UpdateJobDto dto)
    {
        var existingJob = await _context.Jobs
            .Include(j => j.Employer)
            .FirstOrDefaultAsync(j => j.Id == jobId);
        
        if (existingJob == null)
            return null;

        existingJob.Title = dto.Title;
        existingJob.Description = dto.Description;
        existingJob.Location = dto.Location;
        existingJob.JobType = dto.JobType;
        existingJob.Salary = dto.Salary;
        existingJob.Deadline = dto.Deadline;
        existingJob.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return MapToDto(existingJob);
    }

    public async Task<bool> DeleteJobAsync(int jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateJobAsync(int jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return false;

        job.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobDto>> SearchJobsAsync(string searchTerm)
    {
        var jobs = await _context.Jobs
            .Include(j => j.Employer)
            .Where(j => j.IsActive &&
                       (j.Title.Contains(searchTerm) ||
                        j.Description.Contains(searchTerm) ||
                        (j.Location != null && j.Location.Contains(searchTerm))))
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();
        
        return jobs.Select(MapToDto);
    }

    private static JobDto MapToDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            JobType = job.JobType,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            Deadline = job.Deadline,
            IsActive = job.IsActive,
            EmployerId = job.EmployerId,
            EmployerName = job.Employer != null ? $"{job.Employer.FirstName} {job.Employer.LastName}".Trim() : null
        };
    }
}
