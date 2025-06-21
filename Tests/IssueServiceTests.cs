using System.Text.Json;
using HurleyAPI.Models;
using HurleyAPI.Services;
using Xunit;

namespace HurleyAPI.Tests;

public class IssueServiceTests
{
    // Temporary file path for test-generated JSON
    private const string TempFilePath = "temp_issues.json";

    private static JsonSerializerOptions JsonOptions => new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private static List<IssueReport> GetTestIssues() => new()
    {
        new IssueReport
        {
            Id = "12345678",
            Title = "Test Issue",
            Description = "Test Description",
            Severity = IssueSeverity.High,
            Status = IssueStatus.Open,
            CreatedAt = DateTime.UtcNow
        }
    };

    // Test: Verifies that valid JSON correctly loads into IssueService.Issues
    [Fact]
    public void LoadIssuesFromFile_ValidJson_LoadsIssues()
    {
        var json = JsonSerializer.Serialize(GetTestIssues(), JsonOptions);

        File.WriteAllText(TempFilePath, json);

        IssueService.LoadIssuesFromFile(TempFilePath);

        // Assert that exactly one issue is loaded and that the title matches
        Assert.Single(IssueService.Issues);
        Assert.Equal("Test Issue", IssueService.Issues[0].Title);
    }

    // Test: Ensures that attempting to load a non-existent file does not throw an exception
    [Fact]
    public void LoadIssuesFromFile_NonExistentFile_DoesNotThrow()
    {
        var exception = Record.Exception(() => IssueService.LoadIssuesFromFile("nonexistent_file.json"));
        Assert.Null(exception);
    }

    // Test: Verifies that when a valid list of IssueReports is provided
    // the SaveIssuesToFile method creates a file at the given path, writes valid JSON to it,
    // and the contents can be deserialized correctly, preserving the expected data.
    [Fact]
    public void SaveIssuesToFile_CreatesFileWithValidJson()
    {
        IssueService.Issues = GetTestIssues();

        IssueService.SaveIssuesToFile(TempFilePath);

        Assert.True(File.Exists(TempFilePath));

        var content = File.ReadAllText(TempFilePath);
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(content, JsonOptions);

        Assert.NotNull(loaded);
        Assert.Single(loaded);
        Assert.Equal("Test Issue", loaded[0].Title);
    }

    // Test: Verifies that updating an existing issue by ID correctly updates its properties
    [Fact]
    public void UpdateIssueById_ExistingId_UpdatesIssueCorrectly()
    {
        var original = GetTestIssues().First();
        IssueService.Issues = [original];

        var updated = new IssueReport
        {
            Title = "Updated Title",
            Description = "Updated description",
            Severity = IssueSeverity.Critical,
            Status = IssueStatus.Resolved
        };

        var result = IssueService.UpdateIssueById(original.Id, updated);

        // Assert: Ensure the returned result is not null and properties are updated correctly
        Assert.NotNull(result);
        Assert.Equal(original.Id, result.Id);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal(IssueSeverity.Critical, result.Severity);
        Assert.Equal(IssueStatus.Resolved, result.Status);
        Assert.Equal(original.CreatedAt, result.CreatedAt);
        Assert.NotNull(result.ResolvedAt);
    }

    // Cleanup after tests
    ~IssueServiceTests()
    {
        if (File.Exists(TempFilePath))
            File.Delete(TempFilePath);
    }
}