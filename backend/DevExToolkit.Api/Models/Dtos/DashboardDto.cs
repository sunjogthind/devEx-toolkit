namespace DevExToolkit.Api.Models.Dtos;

public class DashboardDto
{
    public int TotalProjects { get; set; }
    public int ActivePipelines { get; set; }
    public int TotalBuilds { get; set; }
    public int ConnectedIntegrations { get; set; }
    public double PipelineSuccessRate { get; set; }
    public double AvgBuildDurationMinutes { get; set; }
    public List<PipelineStatusCount> PipelinesByStatus { get; set; } = new();
    public List<BuildStatusCount> BuildsByStatus { get; set; } = new();
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
}

public class PipelineStatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class BuildStatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
