using LinguaAI.Application.Writing;

namespace LinguaAI.Application.Abstractions;

public interface IWritingProvider
{
    Task<WritingResult> TransformAsync(
        WritingCommand command,
        CancellationToken cancellationToken = default);
}
