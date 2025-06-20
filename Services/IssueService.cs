using System.Text.Json;
using HurleyAPI.Models;

namespace HurleyAPI.Services;

public static class IssueService
{
    public static List<IssueReport> Issues { get; private set; } = [];
    
    // Works with GetAllIssues endpoint
    public static void LoadIssuesFromFile(string path)
    {
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);

        // Set up JSON serialization options
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        // Deserialize the JSON string into a list of IssueReport objects
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(json, options);

        if (loaded is not null)
            Issues = loaded;
    }
    
    // Works with CreateIssue endpoint
    public static void SaveIssuesToFile(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        var json = JsonSerializer.Serialize(Issues, options);
        File.WriteAllText(path, json);
    }
    
    // Works with UpdateIssue endpoint
    public static IssueReport? UpdateIssueById(string id, IssueReport updatedIssue)
    {
        var existing = Issues.FirstOrDefault(i => i.Id == id);
        if (existing is null) return null;
        
        // Keep existing ID and CreatedAt timestamp
        updatedIssue.Id = existing.Id;
        updatedIssue.CreatedAt = existing.CreatedAt;
        
        updatedIssue.ResolvedAt = updatedIssue.Status == IssueStatus.Resolved 
            ? DateTime.UtcNow
            : null;
        
        var index = Issues.IndexOf(existing);
        Issues[index] = updatedIssue;
        
        SaveIssuesToFile("Data/issues.json");
        return updatedIssue;
    }

    // Works with DeleteIssueById endpoint
    public static bool DeleteIssueById(string id)
    {
        var issues = Issues.FirstOrDefault(i => i.Id == id);
        if (issues is null) return false;
        
        Issues.Remove(issues);
        SaveIssuesToFile("Data/issues.json");
        return true;
    }
}