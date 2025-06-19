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
        Title = "HurleyAPI",
        Version = "v1",
        Description = "A RESTful API built with .NET for demonstration and development purposes."
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
app.MapGet("/", () => Results.Ok("Welcome to the Hurley API."))
    .WithName("GetRoot")
    .WithDescription("Returns a basic greeting and confirms that the HurleyAPI is operational.")
    .WithTags("General");

app.Run();