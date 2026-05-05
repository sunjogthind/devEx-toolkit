using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models;
using DevExToolkit.Api.Services;

namespace DevExToolkit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IActivityService _activityService;

    public ProjectsController(AppDbContext db, IActivityService activityService)
    {
        _db = db;
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetAll()
    {
        return await _db.Projects
            .Include(p => p.Pipelines)
            .Include(p => p.Builds)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetById(int id)
    {
        var project = await _db.Projects
            .Include(p => p.Pipelines)
            .Include(p => p.Builds)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();
        return project;
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create(Project project)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        await _activityService.LogActivityAsync("system", "created", $"Project '{project.Name}' was created", "DevEx Portal", project.Name);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Project project)
    {
        if (id != project.Id) return BadRequest();

        project.UpdatedAt = DateTime.UtcNow;
        _db.Entry(project).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
            await _activityService.LogActivityAsync("system", "updated", $"Project '{project.Name}' was updated", "DevEx Portal", project.Name);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _db.Projects.AnyAsync(p => p.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        await _activityService.LogActivityAsync("system", "deleted", $"Project '{project.Name}' was deleted", "DevEx Portal", project.Name);

        return NoContent();
    }
}
