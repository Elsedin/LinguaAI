using LinguaAI.Application.Translations;

namespace LinguaAI.Application.Abstractions;

public interface ITranslationProvider
{
    Task<TranslateResult> TranslateAsync(
        TranslateCommand command,
        CancellationToken cancellationToken = default);
}
