using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevExToolkit.Api.Models;

public class Pipeline
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Running, Success, Failed, Cancelled

    [MaxLength(100)]
    public string Branch { get; set; } = "main";

    [MaxLength(100)]
    public string CommitSha { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CommitMessage { get; set; } = string.Empty;

    [MaxLength(100)]
    public string TriggeredBy { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
