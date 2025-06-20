/*
 * HurleyAPI 
 * Developed by John Shields
 */

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Register Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HurleyAPI",
        Version = "v1",
        Description = "A RESTful issue-tracking API for managing issues across teams and projects."
    });
});

var app = builder.Build();

// Enable Swagger middleware in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register all API routes
HurleyAPI.Services.HurleyAPI.Register(app);

app.Run();