using LinguaAI.Infrastructure.Dashboard;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Tests.Dashboard;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_returns_counts_and_top_pairs_for_current_user()
    {
        await using var db = CreateDb();
        var now = DateTime.UtcNow;
        db.TranslationHistories.AddRange(
            CreateHistory("user-a", "en", "bs", now.AddHours(-1), isFavorite: true),
            CreateHistory("user-a", "en", "bs", now.AddHours(-2)),
            CreateHistory("user-a", "de", "en", now.AddHours(-3)),
            CreateHistory("user-b", "en", "bs", now.AddHours(-1)));
        db.DictionaryEntries.AddRange(
            CreateDictionary("user-a", "one"),
            CreateDictionary("user-a", "two"),
            CreateDictionary("user-b", "other"));
        await db.SaveChangesAsync();

        var service = new DashboardService(db);
        var summary = await service.GetSummaryAsync("user-a");

        Assert.Equal(3, summary.TotalTranslations);
        Assert.Equal(1, summary.FavoriteCount);
        Assert.Equal(2, summary.DictionaryEntryCount);
        Assert.Equal(2, summary.TopLanguagePairs[0].Count);
        Assert.Equal("en", summary.TopLanguagePairs[0].SourceLanguage);
        Assert.Equal("bs", summary.TopLanguagePairs[0].TargetLanguage);
        Assert.Equal(3, summary.RecentActivity.Count);
        Assert.Equal(14, summary.ActivityByDay.Count);
        Assert.True(summary.ActivityByDay.Sum(x => x.Count) >= 3);
    }

    [Fact]
    public async Task GetSummaryAsync_truncates_recent_previews()
    {
        await using var db = CreateDb();
        var longText = new string('a', 120);
        db.TranslationHistories.Add(CreateHistory("user-a", "en", "bs", DateTime.UtcNow, longText, longText));
        await db.SaveChangesAsync();

        var service = new DashboardService(db);
        var summary = await service.GetSummaryAsync("user-a");

        Assert.Single(summary.RecentActivity);
        Assert.True(summary.RecentActivity[0].SourcePreview.Length < longText.Length);
        Assert.EndsWith("…", summary.RecentActivity[0].SourcePreview);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static TranslationHistory CreateHistory(
        string userId,
        string sourceLanguage,
        string targetLanguage,
        DateTime createdAtUtc,
        string sourceText = "hello",
        string translatedText = "zdravo",
        bool isFavorite = false) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        SourceText = sourceText,
        TranslatedText = translatedText,
        SourceLanguage = sourceLanguage,
        TargetLanguage = targetLanguage,
        Style = "natural",
        OperationType = "translation",
        IsFavorite = isFavorite,
        CreatedAtUtc = createdAtUtc
    };

    private static DictionaryEntry CreateDictionary(string userId, string term) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Term = term,
        Translation = "translated",
        SourceLanguage = "en",
        TargetLanguage = "bs",
        CreatedAtUtc = DateTime.UtcNow
    };
}
