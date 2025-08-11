using HurleyAPI.Services;
using Microsoft.OpenApi.Models;
using Supabase;
using System.Text.Json.Serialization;
using ApiService = HurleyAPI.Services.HurleyAPI;

var builder = WebApplication.CreateBuilder(args);

// Read and validate env vars
var url = Environment.GetEnvironmentVariable("SUPABASE_URL")?.Trim();
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY")?.Trim();

if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("https://") || !url.Contains(".supabase.co"))
{
    throw new InvalidOperationException(
        $"Invalid SUPABASE_URL. Got: '{url ?? "<null>"}'. Expected format: https://<projectRef>.supabase.co");
}

if (string.IsNullOrWhiteSpace(key))
{
    throw new InvalidOperationException("SUPABASE_KEY is missing.");
}

// Configure Supabase client
var options = new SupabaseOptions
{
    AutoConnectRealtime = false,
    AutoRefreshToken = true
};

var supabase = new Client(url, key, options);

try
{
    await supabase.InitializeAsync(); // will not try websocket if AutoConnectRealtime=false
}
catch (Exception ex)
{
    // Log the root cause clearly (e.g., DNS, TLS, proxy)
    Console.WriteLine("Failed to initialize Supabase client:");
    Console.WriteLine(ex);
    throw;
}

// Register services and build the API
IssueService.Initialize(supabase);
builder.Services.AddSingleton(supabase);

ConfigureServices(builder.Services);

var app = builder.Build();
ConfigureMiddleware(app);

ApiService.Register(app);

app.Run();
return;

static void ConfigureServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();

    services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HurleyAPI",
            Version = "v1",
            Description = "A RESTful issue-tracking API powered by Supabase"
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
}