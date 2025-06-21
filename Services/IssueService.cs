using System.Text.Json;
using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class IssueService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public static List<IssueReport> Issues { get; private set; } = [];

    // Load all issues from a local JSON file
    public static void LoadIssuesFromFile(string path)
    {
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(json, JsonOptions);
        if (loaded is not null)
            Issues = loaded;
    }

    // Save current issues to a local JSON file
    public static void SaveIssuesToFile(string path)
    {
        var json = JsonSerializer.Serialize(Issues, JsonOptions);
        File.WriteAllText(path, json);
    }

    // Update an issue by ID
    public static IssueReport? UpdateIssueById(string id, IssueReport updated)
    {
        var existing = Issues.FirstOrDefault(i => i.Id == id);
        if (existing is null) return null;

        updated.Id = existing.Id;
        updated.CreatedAt = existing.CreatedAt;
        updated.ResolvedAt = updated.Status == IssueStatus.Resolved
            ? DateTime.UtcNow
            : null;

        Issues[Issues.IndexOf(existing)] = updated;
        SaveIssuesToFile("Data/issues.json");

        return updated;
    }

    // Delete an issue by ID
    public static bool DeleteIssueById(string id)
    {
        var existing = Issues.FirstOrDefault(i => i.Id == id);
        if (existing is null) return false;

        Issues.Remove(existing);
        SaveIssuesToFile("Data/issues.json");

        return true;
    }
}