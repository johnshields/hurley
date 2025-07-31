using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace HurleyAPI.Models;

[Table("issues")]
public class IssueReport : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("severity")]
    public IssueSeverity Severity { get; set; }

    [Column("status")]
    public IssueStatus Status { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }
}