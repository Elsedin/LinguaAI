using LinguaAI.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinguaAI.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ICollection<TranslationHistory> TranslationHistories { get; set; } = [];
    public ICollection<DictionaryEntry> DictionaryEntries { get; set; } = [];
    public UserPreference? Preference { get; set; }
}
