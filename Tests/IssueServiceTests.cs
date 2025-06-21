using System.Text.Json;
using HurleyAPI.Models;
using HurleyAPI.Services;
using Xunit;

namespace HurleyAPI.Tests;

public class IssueServiceTests
{
    // Temporary file path for test-generated JSON
    private const string TempFilePath = "temp_issues.json";

    // Test: Verifies that valid JSON correctly loads into IssueService.Issues
    [Fact]
    public void LoadIssuesFromFile_ValidJson_LoadsIssues()
    {
        var json = JsonSerializer.Serialize(new List<IssueReport>
        {
            new()
            {
                Id = "12345678",
                Title = "Test Issue",
                Description = "Test Description",
                Severity = IssueSeverity.High,
                Status = IssueStatus.Open,
                CreatedAt = DateTime.UtcNow
            }
        }, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });

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
}