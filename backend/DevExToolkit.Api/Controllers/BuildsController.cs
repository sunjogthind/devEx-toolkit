using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IActivityService _activityService;
    private readonly ISlackService _slackService;

    public BuildsController(AppDbContext db, IActivityService activityService, ISlackService slackService)
    {
        _db = db;
        _activityService = activityService;
        _slackService = slackService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Build>>> GetAll()
    {
        return await _db.Builds
            .Include(b => b.Project)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Build>> GetById(int id)
    {
        var build = await _db.Builds
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (build == null) return NotFound();
        return build;
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<List<Build>>> GetByProject(int projectId)
    {
        return await _db.Builds
            .Where(b => b.ProjectId == projectId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Build>> Create(Build build)
    {
        build.CreatedAt = DateTime.UtcNow;
        _db.Builds.Add(build);
        await _db.SaveChangesAsync();

        var project = await _db.Projects.FindAsync(build.ProjectId);
        await _activityService.LogActivityAsync("build", "started",
            $"Build {build.BuildNumber} started for {build.Platform} ({build.Configuration})",
            "Build System", project?.Name);

        return CreatedAtAction(nameof(GetById), new { id = build.Id }, build);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] BuildStatusUpdate update)
    {
        var build = await _db.Builds.Include(b => b.Project).FirstOrDefaultAsync(b => b.Id == id);
        if (build == null) return NotFound();

        build.Status = update.Status;
        if (update.Status is "Success" or "Failed")
        {
            build.CompletedAt = DateTime.UtcNow;
            build.DurationSeconds = (int)(DateTime.UtcNow - build.CreatedAt).TotalSeconds;
        }

        await _db.SaveChangesAsync();

        await _activityService.LogActivityAsync("build", "status_changed",
            $"Build {build.BuildNumber} is now {update.Status}",
            "Build System", build.Project?.Name);

        await _slackService.SendBuildNotificationAsync(
            build.Project?.Name ?? "Unknown", build.BuildNumber, update.Status);

        return NoContent();
    }
}

public class BuildStatusUpdate
{
    public string Status { get; set; } = string.Empty;
}
