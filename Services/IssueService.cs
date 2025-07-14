using System.Text.Json;
using HurleyAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace HurleyAPI.Services;

public static class IssueService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public static string DataFilePath { get; set; } = "Data/issues.json";
    public static List<IssueReport> Issues { get; set; } = [];

    // Load all issues from a local JSON file
    public static void LoadIssuesFromFile(string path)
    {
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(json, JsonOptions);
        if (loaded is not null)
            Issues = loaded;
    }

    public static async Task<List<IssueReport>> LoadIssuesFromDatabase(
        string connectionString,
        string? id,
        IssueSeverity? severity,
        IssueStatus? status,
        DateTime? createdAfter,
        DateTime? createdBefore)
    {
        await using var connection = new MySqlConnection(connectionString);

        var sql = "SELECT * FROM issues WHERE 1 = 1";
        var parameters = new DynamicParameters();
        
        if (!string.IsNullOrWhiteSpace(id))
        {
            sql += " AND Id = @Id";
            parameters.Add("Id", id);
        }

        if (severity.HasValue)
        {
            sql += " AND Severity = @Severity";
            parameters.Add("Severity", severity.Value.ToString());
        }

        if (status.HasValue)
        {
            sql += " AND Status = @Status";
            parameters.Add("Status", status.Value.ToString());
        }

        if (createdAfter.HasValue)
        {
            sql += " AND CreatedAt >= @CreatedAfter";
            parameters.Add("CreatedAfter", createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            sql += " AND CreatedAt <= @CreatedBefore";
            parameters.Add("CreatedBefore", createdBefore.Value);
        }

        sql += " ORDER BY CreatedAt DESC";

        var results = await connection.QueryAsync<IssueReport>(sql, parameters);
        return results.ToList();
    }

    // Save current issues to a local JSON file
    public static void SaveIssuesToFile(string path)
    {
        var json = JsonSerializer.Serialize(Issues, JsonOptions);
        File.WriteAllText(path, json);
    }
    
    public static void InsertIssueToDatabase(IssueReport issue, string connectionString)
    {
        const string sql = @"
        INSERT INTO issues (id, title, description, severity, status, createdAt, resolvedAt)
        VALUES (@Id, @Title, @Description, @Severity, @Status, @CreatedAt, @ResolvedAt);";

        using var connection = new MySqlConnection(connectionString);
        connection.Execute(sql, issue);
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
        SaveIssuesToFile(DataFilePath);

        return updated;
    }

    // Delete an issue by ID
    public static bool DeleteIssueById(string id)
    {
        var existing = Issues.FirstOrDefault(i => i.Id == id);
        if (existing is null) return false;

        Issues.Remove(existing);
        SaveIssuesToFile(DataFilePath);

        return true;
    }
}