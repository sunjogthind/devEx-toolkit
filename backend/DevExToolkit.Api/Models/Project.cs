using System.ComponentModel.DataAnnotations;

namespace DevExToolkit.Api.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Repository { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(100)]
    public string Team { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Pipeline> Pipelines { get; set; } = new List<Pipeline>();
    public ICollection<Build> Builds { get; set; } = new List<Build>();
}
