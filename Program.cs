using HurleyAPI.Services;
using Microsoft.OpenApi.Models;
using Supabase;
using System.Text.Json.Serialization;
using ApiService = HurleyAPI.Services.HurleyAPI;

var builder = WebApplication.CreateBuilder(args);

var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

var options = new SupabaseOptions
{
    AutoConnectRealtime = true,
    AutoRefreshToken = true
};

var supabase = new Client(url, key, options);
await supabase.InitializeAsync();

// Register services
IssueService.Initialize(supabase);
builder.Services.AddSingleton(supabase);
ConfigureServices(builder.Services);

var app = builder.Build();
ConfigureMiddleware(app);

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

    ApiService.Register(app);
}