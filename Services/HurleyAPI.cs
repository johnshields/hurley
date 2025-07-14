using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class HurleyAPI
{
    // Entry point: register routes and load data
    public static void Register(WebApplication app)
    {
        Root(app);

        // Issues endpoints
        GetAllIssues(app);
        CreateIssue(app);
        UpdateIssueById(app);
        DeleteIssueById(app);
    }

    private static void Root(WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
            {
                message = "Welcome to HurleyAPI.",
                status = "OK",
                version = "v2",
                timestamp = DateTime.UtcNow
            }))
            .WithName("GetRoot")
            .WithDescription("Returns a basic greeting and confirms that HurleyAPI is operational.")
            .WithTags("General");
    }

    private static void GetAllIssues(WebApplication app)
    {
        app.MapGet("/issues", async (
                string? id,
                IssueSeverity? severity,
                IssueStatus? status,
                DateTime? createdAfter,
                DateTime? createdBefore) =>
            {
                var issues = await IssueService.LoadIssuesFromDatabase(
                    app.Configuration.GetConnectionString("DefaultConnection")!, 
                    id, severity, status, createdAfter, createdBefore);

                return Results.Ok(issues);
            })
            .WithName("GetAllIssues")
            .WithDescription("Retrieves all issues or filters by severity, status, or creation date.")
            .WithTags("Issues")
            .Produces<List<IssueReport>>();
    }

    private static void CreateIssue(WebApplication app)
    {
        app.MapPost("/issues", (IssueReport newIssue) =>
            {
                newIssue.Id = Guid.NewGuid().ToString("N")[..8];
                newIssue.CreatedAt = DateTime.UtcNow;
                
                IssueService.InsertIssueToDatabase(newIssue);

                return Results.Created($"/issues/{newIssue.Id}", newIssue);
            })
            .WithName("CreateIssue")
            .WithDescription("Creates a new issue.")
            .WithTags("Issues")
            .Produces<IssueReport>(StatusCodes.Status201Created);
    }

    private static void UpdateIssueById(WebApplication app)
    {
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
    }

    private static void DeleteIssueById(WebApplication app)
    {
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
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}