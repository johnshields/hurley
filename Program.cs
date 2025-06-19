using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok("Welcome to the Hurley API."))
    .WithName("GetRoot")
    .WithDescription("Returns a basic greeting and confirms that the Hurley API is operational.")
    .WithTags("General");

app.Run();