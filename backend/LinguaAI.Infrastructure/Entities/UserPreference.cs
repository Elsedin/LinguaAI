using LinguaAI.Infrastructure.Identity;

namespace LinguaAI.Infrastructure.Entities;

public class UserPreference
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public string? PreferredSourceLanguage { get; set; }
    public string? PreferredTargetLanguage { get; set; }
    public string? PreferredStyle { get; set; }
}
