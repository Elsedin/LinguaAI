using LinguaAI.Application.Preferences;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using LinguaAI.Infrastructure.Preferences;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Tests.Preferences;

public class PreferenceServiceTests
{
    [Fact]
    public async Task GetAsync_returns_empty_when_missing()
    {
        await using var db = CreateDb();
        var service = new PreferenceService(db);

        var result = await service.GetAsync("user-a");

        Assert.Null(result.PreferredSourceLanguage);
        Assert.Null(result.PreferredTargetLanguage);
        Assert.Null(result.PreferredStyle);
    }

    [Fact]
    public async Task UpsertAsync_creates_and_updates_preferences()
    {
        await using var db = CreateDb();
        var service = new PreferenceService(db);

        var created = await service.UpsertAsync("user-a", new UpdatePreferenceRequest
        {
            PreferredSourceLanguage = "EN",
            PreferredTargetLanguage = "BS",
            PreferredStyle = "Formal"
        });

        Assert.Equal("en", created.PreferredSourceLanguage);
        Assert.Equal("bs", created.PreferredTargetLanguage);
        Assert.Equal("formal", created.PreferredStyle);
        Assert.Single(db.UserPreferences);

        var updated = await service.UpsertAsync("user-a", new UpdatePreferenceRequest
        {
            PreferredSourceLanguage = "auto",
            PreferredTargetLanguage = "de",
            PreferredStyle = "natural"
        });

        Assert.Equal("auto", updated.PreferredSourceLanguage);
        Assert.Equal("de", updated.PreferredTargetLanguage);
        Assert.Equal("natural", updated.PreferredStyle);
        Assert.Single(db.UserPreferences);
    }

    [Fact]
    public async Task UpsertAsync_rejects_unsupported_style()
    {
        await using var db = CreateDb();
        var service = new PreferenceService(db);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpsertAsync("user-a", new UpdatePreferenceRequest
            {
                PreferredStyle = "poetic"
            }));

        Assert.Contains("style", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAsync_returns_only_current_user_preferences()
    {
        await using var db = CreateDb();
        db.UserPreferences.Add(new UserPreference
        {
            Id = Guid.NewGuid(),
            UserId = "user-b",
            PreferredTargetLanguage = "fr",
            PreferredStyle = "literal"
        });
        await db.SaveChangesAsync();

        var service = new PreferenceService(db);
        var result = await service.GetAsync("user-a");

        Assert.Null(result.PreferredTargetLanguage);
        Assert.Null(result.PreferredStyle);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
