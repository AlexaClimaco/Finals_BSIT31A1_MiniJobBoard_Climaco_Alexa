using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniJobBoard.Application.Jobs;
using MiniJobBoard.Application.Jobs.Dtos;
using MiniJobBoard.Infrastructure.Entities;
using MiniJobBoard.Infrastructure.Services;
using MiniJobBoard.Web.Models;

namespace MiniJobBoard.Web.Controllers;

public class JobsController : Controller
{
    private readonly IJobService _jobService;
    private readonly JobApplicationService _applicationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IJobService jobService,
        JobApplicationService applicationService,
        UserManager<ApplicationUser> userManager,
        ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _applicationService = applicationService;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Jobs
    public async Task<IActionResult> Index(string? searchTerm)
    {
        IEnumerable<JobDto> jobs;
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            jobs = await _jobService.SearchJobsAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
        }
        else
        {
            jobs = await _jobService.GetActiveJobsAsync();
        }

        return View(jobs);
    }

    // GET: Jobs/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null)
        {
            return NotFound();
        }

        // Check if current user has already applied
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                ViewBag.HasApplied = await _applicationService.HasUserAppliedAsync(userId, id);
            }
        }

        return View(job);
    }

    // GET: Jobs/Create
    [Authorize]
    public IActionResult Create()
    {
        var user = _userManager.GetUserAsync(User).Result;
        
        if (user?.Role != UserRole.Employer)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        return View();
    }

    // POST: Jobs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CreateJobViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.Role != UserRole.Employer)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (ModelState.IsValid)
        {
            var createJobDto = new CreateJobDto
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                JobType = model.JobType,
                Salary = model.Salary,
                Deadline = model.Deadline,
                EmployerId = user.Id
            };

            var job = await _jobService.CreateJobAsync(createJobDto);
            _logger.LogInformation($"Job {job.Id} created by employer {user.Id}");
            
            return RedirectToAction(nameof(MyJobs));
        }

        return View(model);
    }

    // GET: Jobs/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (job.EmployerId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var model = new CreateJobViewModel
        {
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            JobType = job.JobType,
            Salary = job.Salary,
            Deadline = job.Deadline
        };

        return View(model);
    }

    // POST: Jobs/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, CreateJobViewModel model)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (job.EmployerId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (ModelState.IsValid)
        {
            var updateJobDto = new UpdateJobDto
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                JobType = model.JobType,
                Salary = model.Salary,
                Deadline = model.Deadline,
                IsActive = job.IsActive
            };

            await _jobService.UpdateJobAsync(id, updateJobDto);
            _logger.LogInformation($"Job {id} updated by employer {userId}");
            
            return RedirectToAction(nameof(MyJobs));
        }

        return View(model);
    }

    // GET: Jobs/MyJobs
    [Authorize]
    public async Task<IActionResult> MyJobs()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.Role != UserRole.Employer)
        {
            return RedirectToAction("Index");
        }

        var jobs = await _jobService.GetJobsByEmployerAsync(user.Id);
        return View(jobs);
    }

    // POST: Jobs/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (job.EmployerId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        await _jobService.DeleteJobAsync(id);
        _logger.LogInformation($"Job {id} deleted by employer {userId}");
        
        return RedirectToAction(nameof(MyJobs));
    }

    // GET: Jobs/Applications/5
    [Authorize]
    public async Task<IActionResult> Applications(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (job.EmployerId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var applications = await _applicationService.GetApplicationsByJobAsync(id);
        ViewBag.Job = job;
        
        return View(applications);
    }
}
