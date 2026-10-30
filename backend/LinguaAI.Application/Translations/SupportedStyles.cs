namespace LinguaAI.Application.Translations;

public static class SupportedStyles
{
    public static readonly IReadOnlySet<string> Values = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "natural",
        "literal",
        "formal",
        "informal",
        "professional",
        "academic"
    };

    public static bool IsValid(string? style) =>
        !string.IsNullOrWhiteSpace(style) && Values.Contains(style);
}
