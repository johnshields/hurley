/*
 * HurleyAPI
 * Developed by John Shields
 */

using ApiService = HurleyAPI.Services.HurleyAPI;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services);

// Build the app
var app = builder.Build();

// Configure middleware and endpoints
ConfigureMiddleware(app);
app.Run();
return;

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

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Register API routes
    ApiService.Register(app);
}