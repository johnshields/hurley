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
                IssueSeverity? severity,
                IssueStatus? status,
                DateTime? createdAfter,
                DateTime? createdBefore) =>
            {
                var issues = await IssueService.LoadAllIssues(severity, status, createdAfter, createdBefore);

                var cleanIssues = issues.Select(issue => new IssueDto(
                    issue.Id,
                    issue.Title,
                    issue.Description,
                    issue.Severity,
                    issue.Status,
                    issue.CreatedAt,
                    issue.ResolvedAt
                ));

                return Results.Ok(cleanIssues);
            })
            .WithName("GetAllIssues")
            .WithDescription("Retrieves all issues or filters by severity, status, or creation date range.")
            .WithTags("Issues")
            .Produces<List<IssueDto>>();
    }

    private static void GetIssueById(WebApplication app)
    {
        app.MapGet("/issues/{id:guid}", async (Guid id) =>
            {
                var issue = await IssueService.LoadIssueById(id);
                if (issue is null)
                    return Results.NotFound(new { error = $"Issue with ID '{id}' not found." });

                var dto = new IssueDto(
                    issue.Id,
                    issue.Title,
                    issue.Description,
                    issue.Severity,
                    issue.Status,
                    issue.CreatedAt,
                    issue.ResolvedAt
                );

                return Results.Ok(dto);
            })
            .WithName("GetIssueById")
            .WithDescription("Retrieves a single issue by its unique ID.")
            .WithTags("Issues")
            .Produces<IssueDto>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static void CreateIssue(WebApplication app)
    {
        app.MapPost("/issues", async (CreateIssueDto newIssue) =>
            {
                var issue = new IssueReport
                {
                    Title = newIssue.Title,
                    Description = newIssue.Description,
                    Severity = newIssue.Severity,
                    Status = newIssue.Status,
                    CreatedAt = DateTime.UtcNow,
                    ResolvedAt = null
                };

                var created = await IssueService.InsertNewIssue(issue);
                if (!created)
                {
                    return Results.Problem("Failed to insert new issue into database.");
                }

                return Results.Created($"/issues/{issue.Id}", new { message = "Issue created." });
            })
            .WithName("CreateIssue")
            .WithDescription("Creates a new issue and stores it in the database.")
            .WithTags("Issues")
            .Accepts<CreateIssueDto>("application/json")
            .Produces<IssueDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static void UpdateIssueById(WebApplication app)
    {
        app.MapPut("/issues/{id:guid}", async (Guid id, UpdateIssueDto updated) =>
            {
                var success = await IssueService.UpdateIssueById(id, updated);
                if (!success)
                {
                    return Results.NotFound(new { error = $"Issue with ID '{id}' not found." });
                }

                return Results.NoContent();
            })
            .WithName("UpdateIssueById")
            .WithDescription("Updates an existing issue.")
            .WithTags("Issues")
            .Accepts<UpdateIssueDto>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static void DeleteIssueById(WebApplication app)
    {
        app.MapDelete("/issues/{id:guid}", async (Guid id) =>
            {
                var deleted = await IssueService.DeleteIssueById(id);
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