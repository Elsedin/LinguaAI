using LinguaAI.Application.Dashboard;
using LinguaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Infrastructure.Dashboard;

public sealed class DashboardService(AppDbContext db) : IDashboardService
{
    private const int TopPairsLimit = 5;
    private const int RecentLimit = 5;
    private const int ActivityDays = 14;
    private const int PreviewLength = 80;

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var history = db.TranslationHistories.Where(x => x.UserId == userId);
        var totalTranslations = await history.CountAsync(cancellationToken);
        var favoriteCount = await history.CountAsync(x => x.IsFavorite, cancellationToken);
        var dictionaryEntryCount = await db.DictionaryEntries
            .CountAsync(x => x.UserId == userId, cancellationToken);

        var topLanguagePairs = await history
            .GroupBy(x => new { x.SourceLanguage, x.TargetLanguage })
            .Select(g => new LanguagePairStatDto
            {
                SourceLanguage = g.Key.SourceLanguage,
                TargetLanguage = g.Key.TargetLanguage,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(TopPairsLimit)
            .ToListAsync(cancellationToken);

        var sinceUtc = DateTime.UtcNow.Date.AddDays(-(ActivityDays - 1));
        var rawActivity = await history
            .Where(x => x.CreatedAtUtc >= sinceUtc)
            .GroupBy(x => x.CreatedAtUtc.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var activityLookup = rawActivity.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);
        var activityByDay = new List<ActivityDayDto>(ActivityDays);
        for (var offset = 0; offset < ActivityDays; offset++)
        {
            var day = DateOnly.FromDateTime(sinceUtc.AddDays(offset));
            activityByDay.Add(new ActivityDayDto
            {
                Date = day,
                Count = activityLookup.GetValueOrDefault(day)
            });
        }

        var recentRows = await history
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(RecentLimit)
            .Select(x => new
            {
                x.Id,
                x.SourceText,
                x.TranslatedText,
                x.SourceLanguage,
                x.TargetLanguage,
                x.Style,
                x.IsFavorite,
                x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        var recentActivity = recentRows
            .Select(x => new RecentActivityDto
            {
                Id = x.Id,
                SourcePreview = Truncate(x.SourceText),
                TranslatedPreview = Truncate(x.TranslatedText),
                SourceLanguage = x.SourceLanguage,
                TargetLanguage = x.TargetLanguage,
                Style = x.Style,
                IsFavorite = x.IsFavorite,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalTranslations = totalTranslations,
            FavoriteCount = favoriteCount,
            DictionaryEntryCount = dictionaryEntryCount,
            TopLanguagePairs = topLanguagePairs,
            ActivityByDay = activityByDay,
            RecentActivity = recentActivity
        };
    }

    private static string Truncate(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= PreviewLength)
            return value;

        return value[..PreviewLength].TrimEnd() + "…";
    }
}
