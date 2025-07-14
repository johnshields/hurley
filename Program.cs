/*
 * HurleyAPI
 * Developed by John Shields
 */

using HurleyAPI.Services;
using ApiService = HurleyAPI.Services.HurleyAPI;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

IssueService.DbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                                  ?? throw new Exception("Connection string not found");

// Configure services
ConfigureServices(builder.Services);

// Build the app
var app = builder.Build();

// Configure middleware, test DB connection, and register API routes
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
    
    TestDatabaseConnection(app.Configuration);

    ApiService.Register(app); // Register API routes
}

static void TestDatabaseConnection(IConfiguration config)
{
    var connectionString = config.GetConnectionString("DefaultConnection") 
                           ?? throw new Exception("Connection string not found");

    try
    {
        using var connect = new MySqlConnection(connectionString);
        connect.Open();
        Console.WriteLine("success MySQL connect");
        connect.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"fail MySQL connect: {ex.Message}");
    }
}