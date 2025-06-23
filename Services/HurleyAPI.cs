using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class HurleyAPI
{
    private static readonly string DataFilePath = IssueService.DataFilePath;

    // Entry point: register routes and load data
    public static void Register(WebApplication app)
    {
        IssueService.LoadIssuesFromFile(DataFilePath); // Load persisted issues at startup

        Root(app);

        // Issues endpoints
        GetAllIssues(app);
        GetIssueById(app);
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
                version = "v1",
                timestamp = DateTime.UtcNow
            }))
            .WithName("GetRoot")
            .WithDescription("Returns a basic greeting and confirms that HurleyAPI is operational.")
            .WithTags("General");
    }

    private static void GetAllIssues(WebApplication app)
    {
        app.MapGet("/issues", (
                IssueSeverity? severity,
                IssueStatus? status,
                DateTime? createdAfter,
                DateTime? createdBefore) =>
            {
                // Filters the list of issues based on optional query parameters: severity, status and created date
                var filteredIssues = IssueService.Issues
                    .Where(i => !severity.HasValue || i.Severity == severity.Value)
                    .Where(i => !status.HasValue || i.Status == status.Value)
                    .Where(i => !createdAfter.HasValue || i.CreatedAt >= createdAfter.Value)
                    .Where(i => !createdBefore.HasValue || i.CreatedAt <= createdBefore.Value)
                    .ToList();

                return Results.Ok(filteredIssues);
            })
            .WithName("GetAllIssues")
            .WithDescription("Retrieves all issues or filters by severity, status, or creation date.")
            .WithTags("Issues")
            .Produces<List<IssueReport>>();
    }

    private static void GetIssueById(WebApplication app)
    {
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
    }

    private static void CreateIssue(WebApplication app)
    {
        app.MapPost("/issues", (IssueReport newIssue) =>
            {
                newIssue.Id = Guid.NewGuid().ToString("N")[..8];
                newIssue.CreatedAt = DateTime.UtcNow;

                IssueService.Issues.Add(newIssue);
                IssueService.SaveIssuesToFile(DataFilePath);

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