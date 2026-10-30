using LinguaAI.Application.Abstractions;

namespace LinguaAI.Application.Translations;

public sealed class TranslationService(ITranslationProvider provider) : ITranslationService
{
    public const int MaxTextLength = 5000;

    public async Task<TranslateResult> TranslateAsync(
        TranslateRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var text = request.Text?.Trim() ?? string.Empty;
        if (text.Length == 0)
            throw new ArgumentException("Text is required.");
        if (text.Length > MaxTextLength)
            throw new ArgumentException($"Text must be at most {MaxTextLength} characters.");

        var source = request.SourceLanguage?.Trim() ?? string.Empty;
        if (!SupportedLanguages.IsValidSource(source))
            throw new ArgumentException("Source language is not supported.");

        var target = request.TargetLanguage?.Trim() ?? string.Empty;
        if (!SupportedLanguages.IsValidTarget(target))
            throw new ArgumentException("Target language is not supported.");

        if (!string.Equals(source, SupportedLanguages.Auto, StringComparison.OrdinalIgnoreCase)
            && string.Equals(source, target, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Source and target languages must differ.");

        var style = request.Style?.Trim() ?? string.Empty;
        if (!SupportedStyles.IsValid(style))
            throw new ArgumentException("Translation style is not supported.");

        var command = new TranslateCommand(
            text,
            source.ToLowerInvariant(),
            target.ToLowerInvariant(),
            style.ToLowerInvariant());

        return await provider.TranslateAsync(command, cancellationToken);
    }
}
