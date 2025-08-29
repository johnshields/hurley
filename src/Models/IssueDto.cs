namespace HurleyAPI.Models;

public record IssueDto(
    Guid Id,
    string Title,
    string Description,
    IssueSeverity Severity,
    IssueStatus Status,
    DateTime CreatedAt,
    DateTime? ResolvedAt
);

public class CreateIssueDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public IssueStatus Status { get; set; }
}

public class UpdateIssueDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public IssueStatus Status { get; set; }
    public DateTime? ResolvedAt { get; set; } 
}