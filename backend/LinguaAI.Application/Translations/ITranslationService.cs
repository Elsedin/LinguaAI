namespace LinguaAI.Application.Translations;

public interface ITranslationService
{
    Task<TranslateResult> TranslateAsync(
        TranslateRequest request,
        CancellationToken cancellationToken = default);
}
