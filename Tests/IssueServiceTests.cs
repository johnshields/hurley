using System.Text.Json;
using HurleyAPI.Models;
using HurleyAPI.Services;
using Xunit;

namespace HurleyAPI.Tests;

public class IssueServiceTests
{
    // Temporary file path for test JSON
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

    // Verifies that valid JSON correctly loads into IssueService.Issues
    [Fact]
    public void LoadIssuesFromFile_ValidJson_LoadsIssues()
    {
        var json = JsonSerializer.Serialize(GetTestIssues(), JsonOptions);
        File.WriteAllText(TempFilePath, json);

        // Act
        IssueService.LoadIssuesFromFile(TempFilePath);

        // Assert
        Assert.Single(IssueService.Issues);
        Assert.Equal("Test Issue", IssueService.Issues[0].Title);
    }

    // Ensures that attempting to load a non-existent file does not throw
    [Fact]
    public void LoadIssuesFromFile_NonExistentFile_DoesNotThrow()
    {
        // Act
        var exception = Record.Exception(() => IssueService.LoadIssuesFromFile("nonexistent_file.json"));

        // Assert
        Assert.Null(exception);
    }

    // Verifies that issues are saved to file as valid JSON
    [Fact]
    public void SaveIssuesToFile_CreatesFileWithValidJson()
    {
        IssueService.Issues = GetTestIssues();

        // Act
        IssueService.SaveIssuesToFile(TempFilePath);

        // Assert
        Assert.True(File.Exists(TempFilePath));
        var content = File.ReadAllText(TempFilePath);
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(content, JsonOptions);
        Assert.NotNull(loaded);
        Assert.Single(loaded);
        Assert.Equal("Test Issue", loaded[0].Title);
    }

    // Ensures that updating an issue by ID modifies it correctly
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

        // Act
        var result = IssueService.UpdateIssueById(original.Id, updated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(original.Id, result.Id);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal(IssueSeverity.Critical, result.Severity);
        Assert.Equal(IssueStatus.Resolved, result.Status);
        Assert.Equal(original.CreatedAt, result.CreatedAt);
        Assert.NotNull(result.ResolvedAt);
    }

    // Ensures that deleting an existing issue removes it and returns true
    [Fact]
    public void DeleteIssueById_ExistingId_RemovesIssue_AndReturnsTrue()
    {
        var issue = GetTestIssues().First();
        IssueService.Issues = [issue];

        // Act
        var result = IssueService.DeleteIssueById(issue.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(IssueService.Issues);
    }

    // Ensures that attempting to delete a non-existent issue returns false
    [Fact]
    public void DeleteIssueById_NonExistingId_ReturnsFalse_AndKeepsListUnchanged()
    {
        IssueService.Issues = [GetTestIssues().First()];

        // Act
        var result = IssueService.DeleteIssueById("notFound");

        // Assert
        Assert.False(result);
        Assert.Single(IssueService.Issues);
    }

    // Ensures that deletion is persisted by verifying the updated file contents
    [Fact]
    public void DeleteIssueById_UpdatesJsonFile()
    {
        var issue = GetTestIssues().First();
        const string filePath = "Data/issues.json";

        if (File.Exists(filePath)) File.Delete(filePath);

        IssueService.Issues = [issue];
        IssueService.SaveIssuesToFile(filePath);

        // Act
        IssueService.DeleteIssueById(issue.Id);

        // Assert
        var loaded = JsonSerializer.Deserialize<List<IssueReport>>(File.ReadAllText(filePath), JsonOptions);
        Assert.NotNull(loaded);
        Assert.Empty(loaded);
    }

    // Ensures filtering the issues by IssueSeverity return the correct subset
    [Fact]
    public void FilterIssuesBySeverity_ReturnsCorrectSubset()
    {
        IssueService.Issues = GetTestIssues();
        var severityFilter = IssueSeverity.High;

        // Act
        var filtered = IssueService.Issues
            .Where(i => i.Severity == severityFilter)
            .ToList();

        // Assert
        Assert.Single(filtered);
        Assert.All(filtered, issue => Assert.Equal(IssueSeverity.High, issue.Severity));
    }

    // Cleanup temp test file
    ~IssueServiceTests()
    {
        if (File.Exists(TempFilePath))
            File.Delete(TempFilePath);
    }
}