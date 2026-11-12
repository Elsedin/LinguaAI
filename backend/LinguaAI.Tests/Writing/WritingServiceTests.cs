using LinguaAI.Application.Abstractions;
using LinguaAI.Application.Writing;

namespace LinguaAI.Tests.Writing;

public class WritingServiceTests
{
    [Fact]
    public async Task TransformAsync_rejects_unsupported_operation()
    {
        var service = new WritingService(new FakeWritingProvider());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TransformAsync(new WritingRequest
            {
                Text = "Hello world",
                Operation = "summarize"
            }));

        Assert.Contains("operation", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TransformAsync_rejects_empty_text()
    {
        var service = new WritingService(new FakeWritingProvider());

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TransformAsync(new WritingRequest
            {
                Text = "   ",
                Operation = WritingOperations.Rewrite
            }));

        Assert.Contains("Text is required", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TransformAsync_calls_provider_with_normalized_values()
    {
        var fake = new FakeWritingProvider();
        var service = new WritingService(fake);

        var result = await service.TransformAsync(new WritingRequest
        {
            Text = "  Hello there  ",
            Operation = "Rewrite",
            Language = "EN"
        });

        Assert.Equal("rewritten:Hello there", result.ResultText);
        Assert.NotNull(fake.LastCommand);
        Assert.Equal("Hello there", fake.LastCommand!.Text);
        Assert.Equal("rewrite", fake.LastCommand.Operation);
        Assert.Equal("en", fake.LastCommand.Language);
    }

    private sealed class FakeWritingProvider : IWritingProvider
    {
        public WritingCommand? LastCommand { get; private set; }

        public Task<WritingResult> TransformAsync(
            WritingCommand command,
            CancellationToken cancellationToken = default)
        {
            LastCommand = command;
            return Task.FromResult(new WritingResult(
                $"rewritten:{command.Text}",
                command.Operation,
                command.Language));
        }
    }
}
