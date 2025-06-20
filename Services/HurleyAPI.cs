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

        // endpoint - GetIssueById
        app.MapGet("/issues/{id}", (string id) =>
            {
                var issue = IssueService.Issues.FirstOrDefault(x => x.Id == id);
                return issue is not null
                    ? Results.Ok(issue)
                    : Results.NotFound(new { message = $"Issue with ID '{id}' not found." });
            })
            .WithName("GetIssueById")
            .WithDescription("Retrieves a single issue by its unique ID.")
            .WithTags("Issues")
            .Produces<IssueReport>()
            .Produces(StatusCodes.Status404NotFound);

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
        
        // endpoint - UpdateIssueById
        app.MapPut("/issues/{id}", (string id, IssueReport updatedIssue) =>
            {
                var result = IssueService.UpdateIssueById(id, updatedIssue);
                return result is null
                    ? Results.NotFound(new { error = $"Issue with ID '{id}' not found." })
                    : Results.Ok(result);
            })
            .WithName("UpdateIssueById")
            .WithDescription("Updates an issue by its unique ID.")
            .WithTags("Issues")
            .Produces<IssueReport>()
            .Produces(StatusCodes.Status404NotFound);
        
        // endpoint - DeleteIssueById
        app.MapDelete("issues/{id}", (string id) =>
            {
                var deleted = IssueService.DeleteIssueById(id);
                return deleted
                    ? Results.Ok(new { message = $"Issue with ID '{id}' was deleted." })
                    : Results.NotFound(new { error = $"Issue with ID '{id}' not found." });
            })
            .WithName("DeleteIssueById")
            .WithDescription("Deletes an issue by its unique ID.")
            .WithTags("Issues")
            .Produces(200)
            .Produces(404);
    }
}