using Microsoft.EntityFrameworkCore;
using DevExToolkit.Api.Data;
using DevExToolkit.Api.Services;
using DevExToolkit.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Integration services
builder.Services.AddHttpClient<IGitHubService, GitHubService>();
builder.Services.AddHttpClient<ISlackService, SlackService>();
builder.Services.AddHttpClient<IJiraService, JiraService>();
builder.Services.AddScoped<IActivityService, ActivityService>();

// Controllers + JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "DevEx Toolkit API", Version = "v1" });
});

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Middleware pipeline
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();
