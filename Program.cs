/*
 * hurleyAPI
 * Developed by John Shields
 */

using Microsoft.OpenApi.Models;
using Hurley.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Register Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "hurleyAPI",
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
HurleyApi.Register(app);

app.Run();