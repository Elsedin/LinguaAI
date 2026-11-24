namespace LinguaAI.Application.Dashboard;

public sealed class DashboardSummaryDto
{
    public required int TotalTranslations { get; init; }
    public required int FavoriteCount { get; init; }
    public required int DictionaryEntryCount { get; init; }
    public required IReadOnlyList<LanguagePairStatDto> TopLanguagePairs { get; init; }
    public required IReadOnlyList<ActivityDayDto> ActivityByDay { get; init; }
    public required IReadOnlyList<RecentActivityDto> RecentActivity { get; init; }
}

public sealed class LanguagePairStatDto
{
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required int Count { get; init; }
}

public sealed class ActivityDayDto
{
    public required DateOnly Date { get; init; }
    public required int Count { get; init; }
}

public sealed class RecentActivityDto
{
    public required Guid Id { get; init; }
    public required string SourcePreview { get; init; }
    public required string TranslatedPreview { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string Style { get; init; }
    public required bool IsFavorite { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
