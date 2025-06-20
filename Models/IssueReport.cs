namespace HurleyAPI.Models;

public class IssueReport
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; } = IssueSeverity.Unknown;
    public IssueStatus Status { get; set; } = IssueStatus.Unknown;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}