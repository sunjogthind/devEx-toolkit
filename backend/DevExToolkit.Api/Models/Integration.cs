using System.ComponentModel.DataAnnotations;

namespace DevExToolkit.Api.Models;

public class Integration
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = string.Empty; // GitHub, GitLab, Slack, JIRA

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Disconnected"; // Connected, Disconnected, Error

    [MaxLength(500)]
    public string ConfigurationJson { get; set; } = "{}";

    [MaxLength(200)]
    public string WebhookUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSyncAt { get; set; } = DateTime.UtcNow;
}
