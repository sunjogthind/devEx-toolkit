using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IActivityService _activityService;
    private readonly IGitHubService _gitHubService;
    private readonly IJiraService _jiraService;

    public IntegrationsController(AppDbContext db, IActivityService activityService,
        IGitHubService gitHubService, IJiraService jiraService)
    {
        _db = db;
        _activityService = activityService;
        _gitHubService = gitHubService;
        _jiraService = jiraService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Integration>>> GetAll()
    {
        return await _db.Integrations.OrderBy(i => i.Provider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Integration>> GetById(int id)
    {
        var integration = await _db.Integrations.FindAsync(id);
        if (integration == null) return NotFound();
        return integration;
    }

    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> ToggleConnection(int id)
    {
        var integration = await _db.Integrations.FindAsync(id);
        if (integration == null) return NotFound();

        integration.Status = integration.Status == "Connected" ? "Disconnected" : "Connected";
        integration.LastSyncAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _activityService.LogActivityAsync("integration", integration.Status.ToLower(),
            $"{integration.Provider} integration {integration.Status.ToLower()}",
            integration.Provider);

        return Ok(integration);
    }

    [HttpGet("github/{owner}/{repo}")]
    public async Task<ActionResult> GetGitHubRepo(string owner, string repo)
    {
        var info = await _gitHubService.GetRepositoryInfoAsync(owner, repo);
        if (info == null) return NotFound();
        return Ok(info);
    }

    [HttpGet("github/{owner}/{repo}/commits")]
    public async Task<ActionResult> GetGitHubCommits(string owner, string repo)
    {
        var commits = await _gitHubService.GetRecentCommitsAsync(owner, repo);
        return Ok(commits);
    }

    [HttpGet("jira/{projectKey}/issues")]
    public async Task<ActionResult> GetJiraIssues(string projectKey)
    {
        var issues = await _jiraService.GetProjectIssuesAsync(projectKey);
        return Ok(issues);
    }
}
