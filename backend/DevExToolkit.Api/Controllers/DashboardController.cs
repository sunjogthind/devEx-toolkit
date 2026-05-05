using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models.Dtos;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IActivityService _activityService;

    public DashboardController(AppDbContext db, IActivityService activityService)
    {
        _db = db;
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var projects = await _db.Projects.CountAsync();
        var pipelines = await _db.Pipelines.ToListAsync();
        var builds = await _db.Builds.ToListAsync();
        var integrations = await _db.Integrations.CountAsync(i => i.Status == "Connected");

        var completedPipelines = pipelines.Where(p => p.Status is "Success" or "Failed").ToList();
        var successRate = completedPipelines.Count > 0
            ? (double)completedPipelines.Count(p => p.Status == "Success") / completedPipelines.Count * 100
            : 0;

        var completedBuilds = builds.Where(b => b.DurationSeconds > 0).ToList();
        var avgDuration = completedBuilds.Count > 0
            ? completedBuilds.Average(b => b.DurationSeconds) / 60.0
            : 0;

        var recentActivities = await _activityService.GetRecentActivitiesAsync(10);

        return Ok(new DashboardDto
        {
            TotalProjects = projects,
            ActivePipelines = pipelines.Count(p => p.Status == "Running"),
            TotalBuilds = builds.Count,
            ConnectedIntegrations = integrations,
            PipelineSuccessRate = Math.Round(successRate, 1),
            AvgBuildDurationMinutes = Math.Round(avgDuration, 1),
            PipelinesByStatus = pipelines.GroupBy(p => p.Status)
                .Select(g => new PipelineStatusCount { Status = g.Key, Count = g.Count() }).ToList(),
            BuildsByStatus = builds.GroupBy(b => b.Status)
                .Select(g => new BuildStatusCount { Status = g.Key, Count = g.Count() }).ToList(),
            RecentActivity = recentActivities.Select(a => new RecentActivityDto
            {
                Type = a.Type,
                Message = a.Message,
                Source = a.Source,
                Timestamp = a.Timestamp
            }).ToList()
        });
    }
}
