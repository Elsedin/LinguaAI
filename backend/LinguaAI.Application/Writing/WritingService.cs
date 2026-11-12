using LinguaAI.Application.Abstractions;
using LinguaAI.Application.Translations;

namespace LinguaAI.Application.Writing;

public sealed class WritingService(IWritingProvider provider) : IWritingService
{
    public const int MaxTextLength = 5000;

    public async Task<WritingResult> TransformAsync(
        WritingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var text = request.Text?.Trim() ?? string.Empty;
        if (text.Length == 0)
            throw new ArgumentException("Text is required.");
        if (text.Length > MaxTextLength)
            throw new ArgumentException($"Text must be at most {MaxTextLength} characters.");

        var operation = request.Operation?.Trim() ?? string.Empty;
        if (!WritingOperations.IsValid(operation))
            throw new ArgumentException("Writing operation is not supported.");

        string? language = null;
        var rawLanguage = request.Language?.Trim();
        if (!string.IsNullOrEmpty(rawLanguage))
        {
            if (!SupportedLanguages.Codes.Contains(rawLanguage))
                throw new ArgumentException("Language is not supported.");
            language = rawLanguage.ToLowerInvariant();
        }

        var command = new WritingCommand(
            text,
            operation.ToLowerInvariant(),
            language);

        return await provider.TransformAsync(command, cancellationToken);
    }
}
