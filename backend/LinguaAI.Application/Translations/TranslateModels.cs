using System.ComponentModel.DataAnnotations;

namespace LinguaAI.Application.Translations;

public sealed class TranslateRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string SourceLanguage { get; set; } = SupportedLanguages.Auto;

    [Required]
    [MaxLength(16)]
    public string TargetLanguage { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Style { get; set; } = "natural";
}

public sealed record TranslateCommand(
    string Text,
    string SourceLanguage,
    string TargetLanguage,
    string Style);

public sealed record TranslateResult(
    string TranslatedText,
    string SourceLanguage,
    string TargetLanguage,
    string Style);

public sealed class TranslateResponse
{
    public required string TranslatedText { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string Style { get; init; }
}
