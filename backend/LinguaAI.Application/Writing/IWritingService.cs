namespace LinguaAI.Application.Writing;

public interface IWritingService
{
    Task<WritingResult> TransformAsync(
        WritingRequest request,
        CancellationToken cancellationToken = default);
}
