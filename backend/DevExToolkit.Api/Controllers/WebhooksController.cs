using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IActivityService _activityService;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(IActivityService activityService, ILogger<WebhooksController> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHubWebhook([FromBody] JsonElement payload)
    {
        var eventType = Request.Headers["X-GitHub-Event"].FirstOrDefault() ?? "unknown";
        _logger.LogInformation("Received GitHub webhook: {EventType}", eventType);

        var message = eventType switch
        {
            "push" => $"Push event received: {payload.GetProperty("ref").GetString()}",
            "pull_request" => $"PR event: {payload.GetProperty("action").GetString()} - {payload.GetProperty("pull_request").GetProperty("title").GetString()}",
            "workflow_run" => $"Workflow run: {payload.GetProperty("workflow_run").GetProperty("name").GetString()} - {payload.GetProperty("action").GetString()}",
            _ => $"GitHub event: {eventType}"
        };

        await _activityService.LogActivityAsync("webhook", eventType, message, "GitHub");

        return Ok(new { received = true, eventType });
    }

    [HttpPost("gitlab")]
    public async Task<IActionResult> GitLabWebhook([FromBody] JsonElement payload)
    {
        var eventType = payload.TryGetProperty("object_kind", out var kind) ? kind.GetString() ?? "unknown" : "unknown";
        _logger.LogInformation("Received GitLab webhook: {EventType}", eventType);

        await _activityService.LogActivityAsync("webhook", eventType,
            $"GitLab event received: {eventType}", "GitLab");

        return Ok(new { received = true, eventType });
    }

    [HttpPost("slack")]
    public async Task<IActionResult> SlackWebhook([FromBody] JsonElement payload)
    {
        // Handle Slack URL verification challenge
        if (payload.TryGetProperty("challenge", out var challenge))
        {
            return Ok(new { challenge = challenge.GetString() });
        }

        var eventType = payload.TryGetProperty("event", out var evt)
            ? evt.GetProperty("type").GetString() ?? "unknown"
            : "unknown";

        _logger.LogInformation("Received Slack webhook: {EventType}", eventType);

        await _activityService.LogActivityAsync("webhook", eventType,
            $"Slack event received: {eventType}", "Slack");

        return Ok(new { received = true });
    }

    [HttpPost("jira")]
    public async Task<IActionResult> JiraWebhook([FromBody] JsonElement payload)
    {
        var eventType = payload.TryGetProperty("webhookEvent", out var evt) ? evt.GetString() ?? "unknown" : "unknown";
        _logger.LogInformation("Received JIRA webhook: {EventType}", eventType);

        await _activityService.LogActivityAsync("webhook", eventType,
            $"JIRA event received: {eventType}", "JIRA");

        return Ok(new { received = true, eventType });
    }
}
