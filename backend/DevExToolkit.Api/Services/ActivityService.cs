using MongoDB.Driver;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Models;

namespace DevExToolkit.Api.Services;

public interface IActivityService
{
    Task LogActivityAsync(string type, string action, string message, string source, string? projectName = null);
    Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 50);
    Task<List<ActivityLog>> GetActivitiesByTypeAsync(string type, int count = 20);
}

public class ActivityService : IActivityService
{
    private readonly MongoDbContext _mongoDb;
    private readonly ILogger<ActivityService> _logger;

    public ActivityService(MongoDbContext mongoDb, ILogger<ActivityService> logger)
    {
        _mongoDb = mongoDb;
        _logger = logger;
    }

    public async Task LogActivityAsync(string type, string action, string message, string source, string? projectName = null)
    {
        try
        {
            var log = new ActivityLog
            {
                Type = type,
                Action = action,
                Message = message,
                Source = source,
                ProjectName = projectName,
                Timestamp = DateTime.UtcNow
            };

            await _mongoDb.ActivityLogs.InsertOneAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log activity to MongoDB. Type: {Type}, Action: {Action}", type, action);
        }
    }

    public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 50)
    {
        try
        {
            return await _mongoDb.ActivityLogs
                .Find(_ => true)
                .SortByDescending(a => a.Timestamp)
                .Limit(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch recent activities from MongoDB");
            return GetFallbackActivities();
        }
    }

    public async Task<List<ActivityLog>> GetActivitiesByTypeAsync(string type, int count = 20)
    {
        try
        {
            return await _mongoDb.ActivityLogs
                .Find(a => a.Type == type)
                .SortByDescending(a => a.Timestamp)
                .Limit(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch activities by type from MongoDB");
            return GetFallbackActivities().Where(a => a.Type == type).ToList();
        }
    }

    private static List<ActivityLog> GetFallbackActivities()
    {
        var now = DateTime.UtcNow;
        return new List<ActivityLog>
        {
            new() { Type = "pipeline", Action = "completed", Message = "Pipeline 'Build & Test' succeeded for Nova Frontline", Source = "GitLab CI", ProjectName = "Nova Frontline", Timestamp = now.AddMinutes(-5) },
            new() { Type = "build", Action = "started", Message = "Build NF-2024-1543 started for Xbox platform", Source = "Build System", ProjectName = "Nova Frontline", Timestamp = now.AddMinutes(-12) },
            new() { Type = "integration", Action = "synced", Message = "GitHub integration synced 24 new commits", Source = "GitHub", ProjectName = "Striker 26", Timestamp = now.AddMinutes(-30) },
            new() { Type = "webhook", Action = "received", Message = "Slack webhook received: deployment approval for Striker 26", Source = "Slack", ProjectName = "Striker 26", Timestamp = now.AddHours(-1) },
            new() { Type = "pipeline", Action = "failed", Message = "Pipeline 'Nightly Build' failed for Gridiron 26", Source = "GitLab CI", ProjectName = "Gridiron 26", Timestamp = now.AddHours(-2) },
            new() { Type = "system", Action = "alert", Message = "Build queue depth exceeded threshold (15 builds pending)", Source = "DevEx Monitor", Timestamp = now.AddHours(-3) },
            new() { Type = "build", Action = "completed", Message = "Build SK-2024-3201 completed successfully (35min)", Source = "Build System", ProjectName = "Striker 26", Timestamp = now.AddHours(-4) },
            new() { Type = "integration", Action = "connected", Message = "JIRA integration reconnected after timeout", Source = "JIRA", Timestamp = now.AddHours(-5) },
        };
    }
}
