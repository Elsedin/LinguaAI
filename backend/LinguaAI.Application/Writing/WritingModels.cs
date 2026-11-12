using System.ComponentModel.DataAnnotations;

namespace LinguaAI.Application.Writing;

public static class WritingOperations
{
    public const string Rewrite = "rewrite";
    public const string Grammar = "grammar";

    public static readonly IReadOnlySet<string> Supported = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Rewrite,
        Grammar
    };

    public static bool IsValid(string? operation) =>
        !string.IsNullOrWhiteSpace(operation) && Supported.Contains(operation);
}

public sealed class WritingRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Operation { get; set; } = WritingOperations.Rewrite;

    [MaxLength(16)]
    public string? Language { get; set; }
}

public sealed record WritingCommand(
    string Text,
    string Operation,
    string? Language);

public sealed record WritingResult(
    string ResultText,
    string Operation,
    string? Language);

public sealed class WritingResponse
{
    public required string ResultText { get; init; }
    public required string Operation { get; init; }
    public string? Language { get; init; }
}
