using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevExToolkit.Api.Models;

public class Build
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string BuildNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Queued"; // Queued, Building, Success, Failed

    [MaxLength(50)]
    public string Platform { get; set; } = string.Empty; // PC, Xbox, PlayStation, Switch

    [MaxLength(50)]
    public string Configuration { get; set; } = "Release"; // Debug, Release, Profile

    public long ArtifactSizeBytes { get; set; }

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
