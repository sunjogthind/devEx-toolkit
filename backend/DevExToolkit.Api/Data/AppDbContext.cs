using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Models;

namespace DevExToolkit.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<Build> Builds => Set<Build>();
    public DbSet<Integration> Integrations => Set<Integration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>().HasMany(p => p.Pipelines)
            .WithOne(p => p.Project).HasForeignKey(p => p.ProjectId);

        modelBuilder.Entity<Project>().HasMany(p => p.Builds)
            .WithOne(b => b.Project).HasForeignKey(b => b.ProjectId);

        // Seed data — game studio projects
        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Nova Frontline", Description = "Next-gen multiplayer FPS", Repository = "studio/nova-frontline", Status = "Active", Team = "Stockholm Studio" },
            new Project { Id = 2, Name = "Striker 26", Description = "Competitive soccer simulation", Repository = "studio/striker-26", Status = "Active", Team = "Vancouver Studio" },
            new Project { Id = 3, Name = "Gridiron 26", Description = "Pro football simulation", Repository = "studio/gridiron-26", Status = "Active", Team = "Orlando Studio" },
            new Project { Id = 4, Name = "Forge Engine", Description = "Core game engine", Repository = "studio/forge-engine", Status = "Active", Team = "Engine Team" },
            new Project { Id = 5, Name = "Nexus Platform", Description = "Game launcher services", Repository = "studio/nexus-platform", Status = "Active", Team = "Platform Team" }
        );

        modelBuilder.Entity<Pipeline>().HasData(
            new Pipeline { Id = 1, Name = "Build & Test", Status = "Success", Branch = "main", CommitSha = "a1b2c3d", CommitMessage = "Fix player physics collision", TriggeredBy = "john.dev", DurationSeconds = 342, ProjectId = 1 },
            new Pipeline { Id = 2, Name = "Deploy Staging", Status = "Running", Branch = "feature/new-ui", CommitSha = "e4f5g6h", CommitMessage = "Add new stadium rendering", TriggeredBy = "sarah.eng", DurationSeconds = 0, ProjectId = 2 },
            new Pipeline { Id = 3, Name = "Nightly Build", Status = "Failed", Branch = "develop", CommitSha = "i7j8k9l", CommitMessage = "Update roster data pipeline", TriggeredBy = "ci-bot", DurationSeconds = 128, ProjectId = 3 },
            new Pipeline { Id = 4, Name = "Engine Build", Status = "Success", Branch = "main", CommitSha = "m0n1o2p", CommitMessage = "Optimize shader compilation", TriggeredBy = "alex.gpu", DurationSeconds = 1205, ProjectId = 4 },
            new Pipeline { Id = 5, Name = "Integration Tests", Status = "Success", Branch = "main", CommitSha = "q3r4s5t", CommitMessage = "Add OAuth2 flow tests", TriggeredBy = "mike.auth", DurationSeconds = 89, ProjectId = 5 },
            new Pipeline { Id = 6, Name = "Build & Test", Status = "Cancelled", Branch = "hotfix/crash", CommitSha = "u6v7w8x", CommitMessage = "Emergency crash fix", TriggeredBy = "john.dev", DurationSeconds = 45, ProjectId = 1 },
            new Pipeline { Id = 7, Name = "Deploy Production", Status = "Success", Branch = "release/1.2", CommitSha = "y9z0a1b", CommitMessage = "Release 1.2.0", TriggeredBy = "release-bot", DurationSeconds = 567, ProjectId = 2 }
        );

        modelBuilder.Entity<Build>().HasData(
            new Build { Id = 1, BuildNumber = "NF-2024-1542", Status = "Success", Platform = "PC", Configuration = "Release", ArtifactSizeBytes = 52428800000, DurationSeconds = 1800, ProjectId = 1 },
            new Build { Id = 2, BuildNumber = "NF-2024-1543", Status = "Building", Platform = "Xbox", Configuration = "Release", ArtifactSizeBytes = 0, DurationSeconds = 0, ProjectId = 1 },
            new Build { Id = 3, BuildNumber = "SK-2024-3201", Status = "Success", Platform = "PlayStation", Configuration = "Release", ArtifactSizeBytes = 48318382080, DurationSeconds = 2100, ProjectId = 2 },
            new Build { Id = 4, BuildNumber = "GI-2024-0892", Status = "Failed", Platform = "PC", Configuration = "Debug", ArtifactSizeBytes = 0, DurationSeconds = 450, ProjectId = 3 },
            new Build { Id = 5, BuildNumber = "FE-2024-7710", Status = "Success", Platform = "PC", Configuration = "Profile", ArtifactSizeBytes = 1073741824, DurationSeconds = 3600, ProjectId = 4 }
        );

        modelBuilder.Entity<Integration>().HasData(
            new Integration { Id = 1, Provider = "GitHub", Name = "GitHub Enterprise", Status = "Connected", WebhookUrl = "/api/webhooks/github" },
            new Integration { Id = 2, Provider = "Slack", Name = "Studio Slack Workspace", Status = "Connected", WebhookUrl = "/api/webhooks/slack" },
            new Integration { Id = 3, Provider = "JIRA", Name = "JIRA Cloud", Status = "Connected", WebhookUrl = "/api/webhooks/jira" },
            new Integration { Id = 4, Provider = "GitLab", Name = "Engine Team GitLab", Status = "Disconnected", WebhookUrl = "/api/webhooks/gitlab" }
        );
    }
}
