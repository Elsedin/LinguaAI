using LinguaAI.Application.Dictionary;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Dictionary;
using LinguaAI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Tests.Dictionary;

public class DictionaryServiceTests
{
    [Fact]
    public async Task CreateAsync_saves_owned_entry()
    {
        await using var db = CreateDb();
        var service = new DictionaryService(db);

        var created = await service.CreateAsync("user-a", new CreateDictionaryEntryRequest
        {
            Term = "  hello  ",
            Translation = "  zdravo  ",
            SourceLanguage = "EN",
            TargetLanguage = "BS",
            ExampleSentence = " Hello there. ",
            Notes = "  greeting  "
        });

        Assert.Equal("hello", created.Term);
        Assert.Equal("zdravo", created.Translation);
        Assert.Equal("en", created.SourceLanguage);
        Assert.Equal("bs", created.TargetLanguage);
        Assert.Equal("Hello there.", created.ExampleSentence);
        Assert.Equal("greeting", created.Notes);
        Assert.Single(db.DictionaryEntries);
        Assert.Equal("user-a", db.DictionaryEntries.Single().UserId);
    }

    [Fact]
    public async Task ListAsync_returns_only_current_user_rows()
    {
        await using var db = CreateDb();
        db.DictionaryEntries.AddRange(
            CreateRow("user-a", "one"),
            CreateRow("user-b", "secret"),
            CreateRow("user-a", "two"));
        await db.SaveChangesAsync();

        var service = new DictionaryService(db);
        var result = await service.ListAsync("user-a", new DictionaryQuery { Page = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item => Assert.DoesNotContain("secret", item.Term));
        Assert.Equal("two", result.Items[0].Term);
    }

    [Fact]
    public async Task CreateAsync_rejects_unsupported_language()
    {
        await using var db = CreateDb();
        var service = new DictionaryService(db);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync("user-a", new CreateDictionaryEntryRequest
            {
                Term = "hello",
                Translation = "zdravo",
                SourceLanguage = "en",
                TargetLanguage = "xx"
            }));

        Assert.Contains("Target language", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static DictionaryEntry CreateRow(string userId, string term) => new()
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
