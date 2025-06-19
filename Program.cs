/*
 * HurleyAPI
 * Developed by John Shields
 */

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Register API and Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "hurleyAPI",
        Version = "v1",
        Description = "An issue-tracking API for managing bugs across teams and projects."
    });
});

var app = builder.Build();

// Enable Swagger middleware in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map root GET endpoint (health check/greeting)
app.MapGet("/", () => Results.Ok("Welcome to hurleyAPI."))
    .WithName("GetRoot")
    .WithDescription("Returns a basic greeting and confirms that hurleyAPI is operational.")
    .WithTags("General");

app.Run();