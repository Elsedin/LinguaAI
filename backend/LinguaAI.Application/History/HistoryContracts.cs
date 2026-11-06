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

public interface IHistoryService
{
    Task SaveAsync(string userId, HistoryDraft draft, CancellationToken cancellationToken = default);
}
