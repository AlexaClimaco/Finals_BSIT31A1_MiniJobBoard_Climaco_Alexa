using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniJobBoard.Application.Jobs;
using MiniJobBoard.Infrastructure.Entities;
using MiniJobBoard.Infrastructure.Services;
using MiniJobBoard.Web.Models;

namespace MiniJobBoard.Web.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly IJobService _jobService;
    private readonly JobApplicationService _applicationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(
        IJobService jobService,
        JobApplicationService applicationService,
        UserManager<ApplicationUser> userManager,
        ILogger<ApplicationsController> logger)
    {
        _jobService = jobService;
        _applicationService = applicationService;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Applications/Apply/5
    [HttpGet]
    public async Task<IActionResult> Apply(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        
        if (job == null || !job.IsActive)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != UserRole.JobSeeker)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        // Check if user has already applied
        var hasApplied = await _applicationService.HasUserAppliedAsync(user.Id, id);
        if (hasApplied)
        {
            TempData["Error"] = "You have already applied to this job.";
            return RedirectToAction("Details", "Jobs", new { id });
        }

        var model = new ApplyJobViewModel
        {
            JobId = job.Id,
            JobTitle = job.Title
        };

        return View(model);
    }

    // POST: Applications/Apply
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyJobViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.Role != UserRole.JobSeeker)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (ModelState.IsValid)
        {
            // Check if user has already applied
            var hasApplied = await _applicationService.HasUserAppliedAsync(user.Id, model.JobId);
            if (hasApplied)
            {
                TempData["Error"] = "You have already applied to this job.";
                return RedirectToAction("Details", "Jobs", new { id = model.JobId });
            }

            var application = new JobApplication
            {
                ApplicantId = user.Id,
                JobId = model.JobId,
                CoverLetter = model.CoverLetter,
                ResumeUrl = model.ResumeUrl,
                Status = ApplicationStatus.Pending
            };

            await _applicationService.CreateApplicationAsync(application);
            _logger.LogInformation($"Application {application.Id} created by user {user.Id} for job {model.JobId}");
            
            TempData["Success"] = "Your application has been submitted successfully!";
            return RedirectToAction("MyApplications");
        }

        var job = await _jobService.GetJobByIdAsync(model.JobId);
        if (job != null)
        {
            model.JobTitle = job.Title;
        }

        return View(model);
    }

    // GET: Applications/MyApplications
    public async Task<IActionResult> MyApplications()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user?.Role != UserRole.JobSeeker)
        {
            return RedirectToAction("Index", "Jobs");
        }

        var applications = await _applicationService.GetApplicationsByApplicantAsync(user.Id);
        return View(applications);
    }

    // POST: Applications/Withdraw/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        
        if (application == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (application.ApplicantId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        await _applicationService.DeleteApplicationAsync(id);
        _logger.LogInformation($"Application {id} withdrawn by user {userId}");
        
        TempData["Success"] = "Application withdrawn successfully.";
        return RedirectToAction(nameof(MyApplications));
    }

    // POST: Applications/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ApplicationStatus status)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        
        if (application == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var job = await _jobService.GetJobByIdAsync(application.JobId);
        
        if (job?.EmployerId != userId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        await _applicationService.UpdateApplicationStatusAsync(id, status);
        _logger.LogInformation($"Application {id} status updated to {status} by employer {userId}");
        
        TempData["Success"] = "Application status updated successfully.";
        return RedirectToAction("Applications", "Jobs", new { id = application.JobId });
    }
}
