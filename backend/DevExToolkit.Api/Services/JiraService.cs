using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DevExToolkit.Api.Services;

public interface IJiraService
{
    Task<List<JiraIssue>> GetProjectIssuesAsync(string projectKey, int maxResults = 20);
    Task<JiraIssue?> GetIssueAsync(string issueKey);
    Task<bool> CreateIssueAsync(string projectKey, string summary, string description, string issueType = "Task");
}

public class JiraService : IJiraService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JiraService> _logger;
    private readonly string _baseUrl;

    public JiraService(HttpClient httpClient, IConfiguration configuration, ILogger<JiraService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration.GetValue<string>("Jira:BaseUrl") ?? "";

        var email = configuration.GetValue<string>("Jira:Email") ?? "";
        var apiToken = configuration.GetValue<string>("Jira:ApiToken") ?? "";

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(apiToken))
        {
            var authBytes = Encoding.ASCII.GetBytes($"{email}:{apiToken}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        }

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<JiraIssue>> GetProjectIssuesAsync(string projectKey, int maxResults = 20)
    {
        if (string.IsNullOrEmpty(_baseUrl))
        {
            _logger.LogWarning("JIRA base URL not configured");
            return GetMockIssues(projectKey);
        }

        try
        {
            var jql = $"project={projectKey} ORDER BY updated DESC";
            var response = await _httpClient.GetAsync($"{_baseUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&maxResults={maxResults}");
            if (!response.IsSuccessStatusCode) return GetMockIssues(projectKey);

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var issues = new List<JiraIssue>();

            foreach (var element in doc.RootElement.GetProperty("issues").EnumerateArray())
            {
                var fields = element.GetProperty("fields");
                issues.Add(new JiraIssue
                {
                    Key = element.GetProperty("key").GetString() ?? "",
                    Summary = fields.GetProperty("summary").GetString() ?? "",
                    Status = fields.GetProperty("status").GetProperty("name").GetString() ?? "",
                    IssueType = fields.GetProperty("issuetype").GetProperty("name").GetString() ?? "",
                    Priority = fields.TryGetProperty("priority", out var p) ? p.GetProperty("name").GetString() ?? "Medium" : "Medium"
                });
            }

            return issues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch JIRA issues for {ProjectKey}", projectKey);
            return GetMockIssues(projectKey);
        }
    }

    public async Task<JiraIssue?> GetIssueAsync(string issueKey)
    {
        if (string.IsNullOrEmpty(_baseUrl))
        {
            return new JiraIssue { Key = issueKey, Summary = "Mock issue", Status = "In Progress", IssueType = "Task", Priority = "Medium" };
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/rest/api/3/issue/{issueKey}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var fields = doc.RootElement.GetProperty("fields");

            return new JiraIssue
            {
                Key = doc.RootElement.GetProperty("key").GetString() ?? "",
                Summary = fields.GetProperty("summary").GetString() ?? "",
                Status = fields.GetProperty("status").GetProperty("name").GetString() ?? "",
                IssueType = fields.GetProperty("issuetype").GetProperty("name").GetString() ?? "",
                Priority = fields.TryGetProperty("priority", out var p) ? p.GetProperty("name").GetString() ?? "Medium" : "Medium"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch JIRA issue {IssueKey}", issueKey);
            return null;
        }
    }

    public async Task<bool> CreateIssueAsync(string projectKey, string summary, string description, string issueType = "Task")
    {
        if (string.IsNullOrEmpty(_baseUrl))
        {
            _logger.LogWarning("JIRA not configured. Would create: {Summary}", summary);
            return true;
        }

        try
        {
            var payload = new
            {
                fields = new
                {
                    project = new { key = projectKey },
                    summary,
                    description = new { type = "doc", version = 1, content = new[] { new { type = "paragraph", content = new[] { new { type = "text", text = description } } } } },
                    issuetype = new { name = issueType }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/rest/api/3/issue", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create JIRA issue");
            return false;
        }
    }

    private static List<JiraIssue> GetMockIssues(string projectKey)
    {
        return new List<JiraIssue>
        {
            new() { Key = $"{projectKey}-101", Summary = "Implement new matchmaking algorithm", Status = "In Progress", IssueType = "Story", Priority = "High" },
            new() { Key = $"{projectKey}-102", Summary = "Fix memory leak in asset loader", Status = "Open", IssueType = "Bug", Priority = "Critical" },
            new() { Key = $"{projectKey}-103", Summary = "Add telemetry for player sessions", Status = "In Review", IssueType = "Task", Priority = "Medium" },
            new() { Key = $"{projectKey}-104", Summary = "Update CI pipeline for ARM builds", Status = "Done", IssueType = "Task", Priority = "Low" },
            new() { Key = $"{projectKey}-105", Summary = "Optimize texture streaming for open world", Status = "In Progress", IssueType = "Story", Priority = "High" }
        };
    }
}

public class JiraIssue
{
    public string Key { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Status { get; set; } = "";
    public string IssueType { get; set; } = "";
    public string Priority { get; set; } = "";
}
