using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevExToolkit.Api.Models;

public class ActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty; // build, pipeline, integration, webhook, system

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("source")]
    public string Source { get; set; } = string.Empty;

    [BsonElement("projectName")]
    public string? ProjectName { get; set; }

    [BsonElement("metadata")]
    public BsonDocument? Metadata { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
