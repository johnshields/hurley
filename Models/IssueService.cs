using System.Text.Json;

namespace HurleyAPI.Models;

public static class IssueService
{
    public static List<IssueReport> Issues { get; private set; } = [];

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
            Issues  = loaded;
    }

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
}