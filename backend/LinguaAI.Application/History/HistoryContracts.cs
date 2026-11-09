namespace LinguaAI.Application.History;

public sealed class HistoryDraft
{
    public required string SourceText { get; init; }
    public required string TranslatedText { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string Style { get; init; }
    public required string OperationType { get; init; }
}

public sealed class HistoryQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TargetLanguage { get; set; }
    public bool? FavoritesOnly { get; set; }
}

public sealed class HistoryItemDto
{
    public required Guid Id { get; init; }
    public required string SourceText { get; init; }
    public required string TranslatedText { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string Style { get; init; }
    public required string OperationType { get; init; }
    public required bool IsFavorite { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
}

public interface IHistoryService
{
    Task SaveAsync(string userId, HistoryDraft draft, CancellationToken cancellationToken = default);

    Task<PagedResult<HistoryItemDto>> ListAsync(
        string userId,
        HistoryQuery query,
        CancellationToken cancellationToken = default);
}
