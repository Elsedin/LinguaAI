using LinguaAI.Infrastructure.Identity;

namespace LinguaAI.Infrastructure.Entities;

public class TranslationHistory
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public string SourceText { get; set; } = null!;
    public string TranslatedText { get; set; } = null!;
    public string SourceLanguage { get; set; } = null!;
    public string TargetLanguage { get; set; } = null!;
    public string Style { get; set; } = null!;
    public string OperationType { get; set; } = null!;
    public bool IsFavorite { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
