using LinguaAI.Infrastructure.Identity;

namespace LinguaAI.Infrastructure.Entities;

public class DictionaryEntry
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public string Term { get; set; } = null!;
    public string Translation { get; set; } = null!;
    public string SourceLanguage { get; set; } = null!;
    public string TargetLanguage { get; set; } = null!;
    public string? ExampleSentence { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
