using System.Text.Json;

namespace DevExToolkit.Api.Services;

public interface IGitHubService
{
    Task<GitHubRepoInfo?> GetRepositoryInfoAsync(string owner, string repo);
    Task<List<GitHubCommit>> GetRecentCommitsAsync(string owner, string repo, int count = 10);
    Task<List<GitHubPullRequest>> GetOpenPullRequestsAsync(string owner, string repo);
}

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DevExToolkit");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

        var token = configuration.GetValue<string>("GitHub:Token");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {token}");
        }
    }

    public async Task<GitHubRepoInfo?> GetRepositoryInfoAsync(string owner, string repo)
    {
        try
        {
            var response = await _httpClient.GetAsync($"repos/{owner}/{repo}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new GitHubRepoInfo
            {
                FullName = root.GetProperty("full_name").GetString() ?? "",
                Description = root.GetProperty("description").GetString() ?? "",
                Stars = root.GetProperty("stargazers_count").GetInt32(),
                OpenIssues = root.GetProperty("open_issues_count").GetInt32(),
                DefaultBranch = root.GetProperty("default_branch").GetString() ?? "main"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch GitHub repo info for {Owner}/{Repo}", owner, repo);
            return null;
        }
    }

    public async Task<List<GitHubCommit>> GetRecentCommitsAsync(string owner, string repo, int count = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/commits?per_page={count}");
            if (!response.IsSuccessStatusCode) return new List<GitHubCommit>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var commits = new List<GitHubCommit>();

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                commits.Add(new GitHubCommit
                {
                    Sha = element.GetProperty("sha").GetString()?[..7] ?? "",
                    Message = element.GetProperty("commit").GetProperty("message").GetString() ?? "",
                    Author = element.GetProperty("commit").GetProperty("author").GetProperty("name").GetString() ?? "",
                    Date = element.GetProperty("commit").GetProperty("author").GetProperty("date").GetDateTime()
                });
            }

            return commits;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch commits for {Owner}/{Repo}", owner, repo);
            return new List<GitHubCommit>();
        }
    }

    public async Task<List<GitHubPullRequest>> GetOpenPullRequestsAsync(string owner, string repo)
    {
        try
        {
            var response = await _httpClient.GetAsync($"repos/{owner}/{repo}/pulls?state=open");
            if (!response.IsSuccessStatusCode) return new List<GitHubPullRequest>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var prs = new List<GitHubPullRequest>();

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                prs.Add(new GitHubPullRequest
                {
                    Number = element.GetProperty("number").GetInt32(),
                    Title = element.GetProperty("title").GetString() ?? "",
                    Author = element.GetProperty("user").GetProperty("login").GetString() ?? "",
                    CreatedAt = element.GetProperty("created_at").GetDateTime()
                });
            }

            return prs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch PRs for {Owner}/{Repo}", owner, repo);
            return new List<GitHubPullRequest>();
        }
    }
}

public class GitHubRepoInfo
{
    public string FullName { get; set; } = "";
    public string Description { get; set; } = "";
    public int Stars { get; set; }
    public int OpenIssues { get; set; }
    public string DefaultBranch { get; set; } = "main";
}

public class GitHubCommit
{
    public string Sha { get; set; } = "";
    public string Message { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime Date { get; set; }
}

public class GitHubPullRequest
{
    public int Number { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
