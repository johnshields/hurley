using System.Text.Json.Serialization;

namespace HurleyAPI.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IssueSeverity
{
    Unknown,
    Low,
    Medium,
    High,
    Critical
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IssueStatus
{
    Unknown,
    Open,
    InProgress,
    Resolved
}