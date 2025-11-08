using MiniJobBoard.Application.Jobs.Dtos;

namespace MiniJobBoard.Application.Jobs;

public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync();
    Task<IEnumerable<JobDto>> GetActiveJobsAsync();
    Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId);
    Task<JobDto?> GetJobByIdAsync(int jobId);
    Task<JobDto> CreateJobAsync(CreateJobDto job);
    Task<JobDto?> UpdateJobAsync(int jobId, UpdateJobDto job);
    Task<bool> DeleteJobAsync(int jobId);
    Task<bool> DeactivateJobAsync(int jobId);
    Task<IEnumerable<JobDto>> SearchJobsAsync(string searchTerm);
}

