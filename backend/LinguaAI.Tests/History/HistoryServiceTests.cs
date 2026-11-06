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

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static TranslationHistory CreateRow(string userId, string sourceText) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        SourceText = sourceText,
        TranslatedText = "translated",
        SourceLanguage = "en",
        TargetLanguage = "bs",
        Style = "natural",
        OperationType = "translation",
        IsFavorite = false,
        CreatedAtUtc = DateTime.UtcNow
    };
}
