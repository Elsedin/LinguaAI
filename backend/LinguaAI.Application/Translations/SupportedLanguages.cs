namespace LinguaAI.Application.Translations;

public static class SupportedLanguages
{
    public const string Auto = "auto";

    public static readonly IReadOnlySet<string> Codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "bs", "hr", "sr", "en", "de", "fr", "es", "it"
    };

    public static bool IsValidSource(string? code) =>
        !string.IsNullOrWhiteSpace(code)
        && (string.Equals(code, Auto, StringComparison.OrdinalIgnoreCase) || Codes.Contains(code));

    public static bool IsValidTarget(string? code) =>
        !string.IsNullOrWhiteSpace(code)
        && Codes.Contains(code);
}
