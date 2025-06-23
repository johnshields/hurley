/*
 * HurleyAPI
 * Developed by John Shields
 */

using HurleyAPI.Services;
using ApiService = HurleyAPI.Services.HurleyAPI;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Read the IssueService data file path from configuration (fallback to "Data/issues.json" if not set)
IssueService.DataFilePath = builder.Configuration.GetValue<string>("IssueSettings:DataFilePath", "Data/issues.json");

// Configure services
ConfigureServices(builder.Services);

// Build the app
var app = builder.Build();

// Configure middleware and endpoints
ConfigureMiddleware(app);
app.Run();
return;

// Register all required services into the IServiceCollection
static void ConfigureServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();

    services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HurleyAPI",
            Version = "v1",
            Description = "A RESTful issue-tracking API for managing issues across teams and projects."
        });
    });
}

// Set up middleware pipeline and route registration
static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    ApiService.Register(app); // Register API routes
}