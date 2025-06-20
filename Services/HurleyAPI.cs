using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class HurleyAPI
{
    public static void Register(WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
            {
                message = "Welcome to HurleyAPI.",
                status = "OK",
                version = "v1",
                timestamp = DateTime.UtcNow
            }))
            .WithName("GetRoot")
            .WithDescription("Returns a basic greeting and confirms that HurleyAPI is operational.")
            .WithTags("General");
        
        app.MapGet("/issues", () =>
            Results.Ok(IssueService.Issues))
            .WithName("GetAllIssues")
            .WithDescription("Retrieves a list of all issues.")
            .WithTags("Issues");
    }
}