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

        Assert.Equal("result:Hello there", result.ResultText);
        Assert.NotNull(fake.LastCommand);
        Assert.Equal("Hello there", fake.LastCommand!.Text);
        Assert.Equal("rewrite", fake.LastCommand.Operation);
        Assert.Equal("en", fake.LastCommand.Language);
    }

    [Fact]
    public async Task TransformAsync_accepts_grammar_operation()
    {
        var fake = new FakeWritingProvider();
        var service = new WritingService(fake);

        var result = await service.TransformAsync(new WritingRequest
        {
            Text = "She go to school yesterday.",
            Operation = "Grammar"
        });

        Assert.Equal("grammar", result.Operation);
        Assert.Equal("grammar", fake.LastCommand!.Operation);
        Assert.Equal("She go to school yesterday.", fake.LastCommand.Text);
    }

    [Fact]
    public async Task TransformAsync_accepts_simplify_operation()
    {
        var fake = new FakeWritingProvider();
        var service = new WritingService(fake);

        var result = await service.TransformAsync(new WritingRequest
        {
            Text = "The aforementioned methodology facilitates comprehension.",
            Operation = "Simplify"
        });

        Assert.Equal("simplify", result.Operation);
        Assert.Equal("simplify", fake.LastCommand!.Operation);
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
                $"result:{command.Text}",
                command.Operation,
                command.Language));
        }
    }
}
