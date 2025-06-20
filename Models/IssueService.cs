namespace HurleyAPI.Models;

public static class IssueService
{
    public static readonly List<IssueReport> Issues =
    [
        new IssueReport
        {
            Title = "Leaking pipe in basement utility room",
            Description = "Water leak detected during routine inspection. Needs immediate attention before drywall installation.",
            Severity = IssueSeverity.High,
            Status = IssueStatus.Open,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        },

        new IssueReport
        {
            Title = "Missing electrical outlet in office 203",
            Description = "Plans show a power outlet on the north wall - not present during current walkthrough.",
            Severity = IssueSeverity.Medium,
            Status = IssueStatus.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        },

        new IssueReport
        {
            Title = "Cracked tile in lobby floor",
            Description = "One ceramic tile in front entrance is cracked and uneven. Safety concern for foot traffic.",
            Severity = IssueSeverity.Low,
            Status = IssueStatus.Resolved,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            ResolvedAt = DateTime.UtcNow.AddDays(7)
        }
    ];
}