using Microsoft.EntityFrameworkCore;
using MiniJobBoard.Infrastructure.Data;
using MiniJobBoard.Infrastructure.Entities;

namespace MiniJobBoard.Infrastructure.Services;

public class JobApplicationService
{
    private readonly AppDbContext _context;

    public JobApplicationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobApplication>> GetApplicationsByJobAsync(int jobId)
    {
        return await _context.JobApplications
            .Include(a => a.Applicant)
            .Include(a => a.Job)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetApplicationsByApplicantAsync(string applicantId)
    {
        return await _context.JobApplications
            .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
    {
        return await _context.JobApplications
            .Include(a => a.Applicant)
            .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
    }

    public async Task<JobApplication> CreateApplicationAsync(JobApplication application)
    {
        application.AppliedDate = DateTime.UtcNow;
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<JobApplication?> UpdateApplicationStatusAsync(int applicationId, ApplicationStatus status)
    {
        var application = await _context.JobApplications.FindAsync(applicationId);
        if (application == null)
            return null;

        application.Status = status;
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> DeleteApplicationAsync(int applicationId)
    {
        var application = await _context.JobApplications.FindAsync(applicationId);
        if (application == null)
            return false;

        _context.JobApplications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasUserAppliedAsync(string applicantId, int jobId)
    {
        return await _context.JobApplications
            .AnyAsync(a => a.ApplicantId == applicantId && a.JobId == jobId);
    }
}
