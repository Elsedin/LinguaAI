using LinguaAI.Application.History;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using LinguaAI.Infrastructure.History;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Tests.History;

public class HistoryServiceTests
{
    [Fact]
    public async Task ListAsync_returns_only_current_user_rows()
    {
        await using var db = CreateDb();
        db.TranslationHistories.AddRange(
            CreateRow("user-a", "Hello A"),
            CreateRow("user-b", "Hello B"),
            CreateRow("user-a", "Second A"));
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var result = await service.ListAsync("user-a", new HistoryQuery { Page = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item => Assert.DoesNotContain("B", item.SourceText));
        Assert.Equal("Second A", result.Items[0].SourceText);
    }

    [Fact]
    public async Task ListAsync_applies_pagination()
    {
        await using var db = CreateDb();
        for (var i = 0; i < 5; i++)
            db.TranslationHistories.Add(CreateRow("user-a", $"Text {i}"));
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var result = await service.ListAsync("user-a", new HistoryQuery { Page = 2, PageSize = 2 });

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task ListAsync_filters_by_search_text()
    {
        await using var db = CreateDb();
        db.TranslationHistories.AddRange(
            CreateRow("user-a", "Good morning", translatedText: "Dobro jutro"),
            CreateRow("user-a", "Good night", translatedText: "Laku noc"),
            CreateRow("user-a", "Airport", translatedText: "Aerodrom"));
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var result = await service.ListAsync("user-a", new HistoryQuery
        {
            Page = 1,
            PageSize = 10,
            Search = "good"
        });

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item => Assert.Contains("Good", item.SourceText, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ListAsync_filters_by_language_and_favorites()
    {
        await using var db = CreateDb();
        db.TranslationHistories.AddRange(
            CreateRow("user-a", "One", sourceLanguage: "en", targetLanguage: "bs", isFavorite: true),
            CreateRow("user-a", "Two", sourceLanguage: "en", targetLanguage: "de", isFavorite: true),
            CreateRow("user-a", "Three", sourceLanguage: "en", targetLanguage: "bs", isFavorite: false));
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var result = await service.ListAsync("user-a", new HistoryQuery
        {
            Page = 1,
            PageSize = 10,
            SourceLanguage = "en",
            TargetLanguage = "bs",
            FavoritesOnly = true
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("One", result.Items[0].SourceText);
        Assert.True(result.Items[0].IsFavorite);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_flips_owned_row()
    {
        await using var db = CreateDb();
        var row = CreateRow("user-a", "Hello", isFavorite: false);
        db.TranslationHistories.Add(row);
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var first = await service.ToggleFavoriteAsync("user-a", row.Id);
        var second = await service.ToggleFavoriteAsync("user-a", row.Id);

        Assert.NotNull(first);
        Assert.True(first.IsFavorite);
        Assert.NotNull(second);
        Assert.False(second.IsFavorite);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_returns_null_for_other_users_row()
    {
        await using var db = CreateDb();
        var row = CreateRow("user-b", "Secret");
        db.TranslationHistories.Add(row);
        await db.SaveChangesAsync();

        var service = new HistoryService(db);
        var result = await service.ToggleFavoriteAsync("user-a", row.Id);

        Assert.Null(result);
        Assert.False((await db.TranslationHistories.SingleAsync()).IsFavorite);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static TranslationHistory CreateRow(
        string userId,
        string sourceText,
        string translatedText = "translated",
        string sourceLanguage = "en",
        string targetLanguage = "bs",
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
        CreatedAtUtc = DateTime.UtcNow
    };
}
