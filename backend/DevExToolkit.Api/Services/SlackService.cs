using System.Text;
using System.Text.Json;

namespace DevExToolkit.Api.Services;

public interface ISlackService
{
    Task<bool> SendNotificationAsync(string message, string channel = "#devex-alerts");
    Task<bool> SendBuildNotificationAsync(string projectName, string buildNumber, string status);
    Task<bool> SendPipelineNotificationAsync(string projectName, string pipelineName, string status);
}

public class SlackService : ISlackService
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;
    private readonly ILogger<SlackService> _logger;

    public SlackService(HttpClient httpClient, IConfiguration configuration, ILogger<SlackService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _webhookUrl = configuration.GetValue<string>("Slack:WebhookUrl") ?? "";
    }

    public async Task<bool> SendNotificationAsync(string message, string channel = "#devex-alerts")
    {
        if (string.IsNullOrEmpty(_webhookUrl))
        {
            _logger.LogWarning("Slack webhook URL not configured. Message: {Message}", message);
            return false;
        }

        try
        {
            var payload = new { text = message, channel };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_webhookUrl, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Slack notification");
            return false;
        }
    }

    public async Task<bool> SendBuildNotificationAsync(string projectName, string buildNumber, string status)
    {
        var emoji = status switch
        {
            "Success" => ":white_check_mark:",
            "Failed" => ":x:",
            "Building" => ":hammer:",
            _ => ":information_source:"
        };

        var message = $"{emoji} *Build Update*\n*Project:* {projectName}\n*Build:* {buildNumber}\n*Status:* {status}";
        return await SendNotificationAsync(message);
    }

    public async Task<bool> SendPipelineNotificationAsync(string projectName, string pipelineName, string status)
    {
        var emoji = status switch
        {
            "Success" => ":rocket:",
            "Failed" => ":rotating_light:",
            "Running" => ":gear:",
            _ => ":information_source:"
        };

        var message = $"{emoji} *Pipeline Update*\n*Project:* {projectName}\n*Pipeline:* {pipelineName}\n*Status:* {status}";
        return await SendNotificationAsync(message);
    }
}
