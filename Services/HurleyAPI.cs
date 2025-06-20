using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class HurleyAPI
{
    public static void Register(WebApplication app)
    {
        IssueService.LoadIssuesFromFile("Data/issues.json");
        
        // endpoint - GetRoot
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
        
        // endpoint - GetAllIssues
        app.MapGet("/issues", () => Results.Ok(IssueService.Issues))
            .WithName("GetAllIssues")
            .WithDescription("Retrieves a list of all issues.")
            .WithTags("Issues")
            .Produces<List<IssueReport>>();
        
        // endpoint - CreateIssue
        app.MapPost("/issues", (IssueReport newIssue) =>
        {
            newIssue.Id = Guid.NewGuid().ToString("N")[..8];
            newIssue.CreatedAt = DateTime.UtcNow;
            
            IssueService.Issues.Add(newIssue);
            IssueService.SaveIssuesToFile("Data/issues.json");
            
            return Results.Created($"/issues/{newIssue.Id}", newIssue);
        })
        .WithName("CreateIssue")
        .WithDescription("Creates a new issue.")
        .WithTags("Issues")
        .Produces<IssueReport>(StatusCodes.Status201Created);
    }
}