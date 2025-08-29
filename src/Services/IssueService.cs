using HurleyAPI.Models;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using Client = Supabase.Client;

namespace HurleyAPI.Services;

public static class IssueService
{
    private static Client? _supabase;
    private const string Message = "Supabase client not initialised.";

    public static void Initialize(Client supabaseClient)
    {
        _supabase = supabaseClient;
    }

    public static async Task<List<IssueReport>> LoadAllIssues(
        IssueSeverity? severity = null,
        IssueStatus? status = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null)
    {
        if (_supabase == null)
            throw new InvalidOperationException(Message);

        IPostgrestTable<IssueReport> query = _supabase.From<IssueReport>();

        if (severity.HasValue)
            query = query.Filter("severity", Constants.Operator.Equals, severity.ToString());

        if (status.HasValue)
            query = query.Filter("status", Constants.Operator.Equals, status.ToString());

        if (createdAfter.HasValue)
            query = query.Filter("created_at", Constants.Operator.GreaterThanOrEqual, createdAfter.Value.ToString("o"));

        if (createdBefore.HasValue)
            query = query.Filter("created_at", Constants.Operator.LessThanOrEqual, createdBefore.Value.ToString("o"));


        var result = await query.Get();
        return result.Models;
    }

    public static async Task<IssueReport?> LoadIssueById(Guid id)
    {
        if (_supabase == null)
            throw new InvalidOperationException(Message);

        var result = await _supabase
            .From<IssueReport>()
            .Where(x => x.Id == id)
            .Get();

        return result.Models.FirstOrDefault();
    }

    public static async Task<bool> InsertNewIssue(IssueReport issue)
    {
        if (_supabase == null)
            throw new InvalidOperationException(Message);

        var result = await _supabase
            .From<IssueReport>()
            .Insert(issue);

        return result.ResponseMessage is { IsSuccessStatusCode: true };
    }

    public static async Task<bool> UpdateIssueById(Guid id, UpdateIssueDto updatedData)
    {
        if (_supabase == null)
            throw new InvalidOperationException(Message);

        var existing = await LoadIssueById(id);
        if (existing is null)
            return false;

        existing.Title = updatedData.Title;
        existing.Description = updatedData.Description;
        existing.Severity = updatedData.Severity;
        existing.Status = updatedData.Status;
        existing.ResolvedAt = updatedData.ResolvedAt;

        var result = await _supabase.From<IssueReport>().Update(existing);
        return result.ResponseMessage is { IsSuccessStatusCode: true };
    }

    public static async Task<bool> DeleteIssueById(Guid id)
    {
        if (_supabase == null)
            throw new InvalidOperationException(Message);

        var issue = await LoadIssueById(id);
        if (issue is null) return false;

        var result = await _supabase.From<IssueReport>().Delete(issue);
        return result.ResponseMessage is { IsSuccessStatusCode: true };
    }
}