using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PipelinesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IActivityService _activityService;
    private readonly ISlackService _slackService;

    public PipelinesController(AppDbContext db, IActivityService activityService, ISlackService slackService)
    {
        _db = db;
        _activityService = activityService;
        _slackService = slackService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Pipeline>>> GetAll()
    {
        return await _db.Pipelines
            .Include(p => p.Project)
            .OrderByDescending(p => p.StartedAt)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pipeline>> GetById(int id)
    {
        var pipeline = await _db.Pipelines
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pipeline == null) return NotFound();
        return pipeline;
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<List<Pipeline>>> GetByProject(int projectId)
    {
        return await _db.Pipelines
            .Where(p => p.ProjectId == projectId)
            .OrderByDescending(p => p.StartedAt)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Pipeline>> Create(Pipeline pipeline)
    {
        pipeline.StartedAt = DateTime.UtcNow;
        _db.Pipelines.Add(pipeline);
        await _db.SaveChangesAsync();

        var project = await _db.Projects.FindAsync(pipeline.ProjectId);
        await _activityService.LogActivityAsync("pipeline", "started",
            $"Pipeline '{pipeline.Name}' started on branch {pipeline.Branch}",
            "CI/CD", project?.Name);

        return CreatedAtAction(nameof(GetById), new { id = pipeline.Id }, pipeline);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] PipelineStatusUpdate update)
    {
        var pipeline = await _db.Pipelines.Include(p => p.Project).FirstOrDefaultAsync(p => p.Id == id);
        if (pipeline == null) return NotFound();

        pipeline.Status = update.Status;
        if (update.Status is "Success" or "Failed" or "Cancelled")
        {
            pipeline.CompletedAt = DateTime.UtcNow;
            pipeline.DurationSeconds = (int)(DateTime.UtcNow - pipeline.StartedAt).TotalSeconds;
        }

        await _db.SaveChangesAsync();

        await _activityService.LogActivityAsync("pipeline", "status_changed",
            $"Pipeline '{pipeline.Name}' is now {update.Status}",
            "CI/CD", pipeline.Project?.Name);

        await _slackService.SendPipelineNotificationAsync(
            pipeline.Project?.Name ?? "Unknown", pipeline.Name, update.Status);

        return NoContent();
    }
}

public class PipelineStatusUpdate
{
    public string Status { get; set; } = string.Empty;
}
