using HurleyAPI.Models;
using HurleyAPI.Services;
using Supabase;
using Xunit;

namespace HurleyAPI.Tests;

public class IssueServiceTests : IAsyncLifetime
{
    private Client? _supabase;
    private readonly List<Guid> _testIssueIds = new();

    public async Task InitializeAsync()
    {
        var url = "https://usazxzejmarsjcygmkya.supabase.co";
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVzYXp4emVqbWFyc2pjeWdta3lhIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTM5MjI5MzksImV4cCI6MjA2OTQ5ODkzOX0.FlOxqcA7bd_B8Yn8rva6FJutVLjKoduQ1FZ76vuaoHo";

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = false
        };

        _supabase = new Client(url, key, options);
        await _supabase.InitializeAsync();

        IssueService.Initialize(_supabase);
    }

    public async Task DisposeAsync()
    {
        foreach (var id in _testIssueIds)
        {
            await IssueService.DeleteIssueById(id);
        }
    }

    private static IssueReport GenerateTestIssue(string? title = null)
    {
        return new IssueReport
        {
            Id = Guid.NewGuid(),
            Title = title ?? $"Test Issue {Guid.NewGuid()}",
            Description = "Test description",
            Severity = IssueSeverity.Medium,
            Status = IssueStatus.Open,
            CreatedAt = DateTime.UtcNow,
            ResolvedAt = null
        };
    }

    [Fact]
    public async Task LoadAllIssues_DoesNotThrow()
    {
        var result = await IssueService.LoadAllIssues();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoadIssueById_NonExistent_ReturnsNull()
    {
        var result = await IssueService.LoadIssueById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task InsertNewIssue_ReturnsTrue()
    {
        var issue = GenerateTestIssue("Test Insert");

        var result = await IssueService.InsertNewIssue(issue);

        Assert.True(result);

        _testIssueIds.Add(issue.Id);
    }

    [Fact]
    public async Task UpdateIssueById_NonExistent_ReturnsFalse()
    {
        var result = await IssueService.UpdateIssueById(Guid.NewGuid(), new UpdateIssueDto
        {
            Title = "Test Update",
            Description = "No issue with this ID",
            Severity = IssueSeverity.Critical,
            Status = IssueStatus.Open,
            ResolvedAt = null
        });

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteIssueById_NonExistent_ReturnsFalse()
    {
        var randomId = Guid.NewGuid();
        var result = await IssueService.DeleteIssueById(randomId);
        Assert.False(result);
    }
}