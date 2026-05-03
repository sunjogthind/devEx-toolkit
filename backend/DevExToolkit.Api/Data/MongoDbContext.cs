using MongoDB.Driver;
using DevExToolkit.Api.Models;

namespace DevExToolkit.Api.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://localhost:27017";
        var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName") ?? "devex_toolkit";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<ActivityLog> ActivityLogs =>
        _database.GetCollection<ActivityLog>("activity_logs");
}
