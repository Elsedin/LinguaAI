using System.ComponentModel.DataAnnotations;

namespace LinguaAI.Application.Dictionary;

public sealed class DictionaryEntryRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public string Term { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public string Translation { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string SourceLanguage { get; set; } = string.Empty;

    [Required]
    [MaxLength(16)]
    public string TargetLanguage { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? ExampleSentence { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public sealed class DictionaryQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TargetLanguage { get; set; }
}

public sealed class DictionaryEntryDto
{
    public required Guid Id { get; init; }
    public required string Term { get; init; }
    public required string Translation { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public string? ExampleSentence { get; init; }
    public string? Notes { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class PagedDictionaryResult
{
    public required IReadOnlyList<DictionaryEntryDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
}

public interface IDictionaryService
{
    Task<DictionaryEntryDto> CreateAsync(
        string userId,
        DictionaryEntryRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedDictionaryResult> ListAsync(
        string userId,
        DictionaryQuery query,
        CancellationToken cancellationToken = default);

    Task<DictionaryEntryDto?> UpdateAsync(
        string userId,
        Guid id,
        DictionaryEntryRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string userId,
        Guid id,
        CancellationToken cancellationToken = default);
}
