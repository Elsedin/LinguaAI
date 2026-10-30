using LinguaAI.Application.Abstractions;
using LinguaAI.Application.Translations;

namespace LinguaAI.Tests.Translations;

public class TranslationServiceTests
{
    [Fact]
    public async Task TranslateAsync_rejects_unsupported_target_language()
    {
        var service = new TranslationService(new FakeTranslationProvider());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TranslateAsync(new TranslateRequest
            {
                Text = "Hello",
                SourceLanguage = "en",
                TargetLanguage = "xx",
                Style = "natural"
            }));

        Assert.Contains("Target language", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TranslateAsync_rejects_same_source_and_target()
    {
        var service = new TranslationService(new FakeTranslationProvider());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TranslateAsync(new TranslateRequest
            {
                Text = "Hello",
                SourceLanguage = "en",
                TargetLanguage = "en",
                Style = "natural"
            }));

        Assert.Contains("must differ", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TranslateAsync_calls_provider_with_normalized_values()
    {
        var fake = new FakeTranslationProvider();
        var service = new TranslationService(fake);

        var result = await service.TranslateAsync(new TranslateRequest
        {
            Text = "  Ciao  ",
            SourceLanguage = "IT",
            TargetLanguage = "EN",
            Style = "Formal"
        });

        Assert.Equal("translated:Ciao", result.TranslatedText);
        Assert.NotNull(fake.LastCommand);
        Assert.Equal("Ciao", fake.LastCommand!.Text);
        Assert.Equal("it", fake.LastCommand.SourceLanguage);
        Assert.Equal("en", fake.LastCommand.TargetLanguage);
        Assert.Equal("formal", fake.LastCommand.Style);
    }

    private sealed class FakeTranslationProvider : ITranslationProvider
    {
        public TranslateCommand? LastCommand { get; private set; }

        public Task<TranslateResult> TranslateAsync(
            TranslateCommand command,
            CancellationToken cancellationToken = default)
        {
            LastCommand = command;
            return Task.FromResult(new TranslateResult(
                $"translated:{command.Text}",
                command.SourceLanguage,
                command.TargetLanguage,
                command.Style));
        }
    }
}
