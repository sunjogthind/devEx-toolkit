using Microsoft.AspNetCore.Mvc;
using DevExToolkit.Api.Models;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActivityLog>>> GetRecent([FromQuery] int count = 50)
    {
        var activities = await _activityService.GetRecentActivitiesAsync(count);
        return Ok(activities);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<List<ActivityLog>>> GetByType(string type, [FromQuery] int count = 20)
    {
        var activities = await _activityService.GetActivitiesByTypeAsync(type, count);
        return Ok(activities);
    }
}
