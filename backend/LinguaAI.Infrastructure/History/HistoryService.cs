using LinguaAI.Application.History;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;

namespace LinguaAI.Infrastructure.History;

public sealed class HistoryService(AppDbContext db) : IHistoryService
{
    public async Task SaveAsync(string userId, HistoryDraft draft, CancellationToken cancellationToken = default)
    {
        db.TranslationHistories.Add(new TranslationHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SourceText = draft.SourceText,
            TranslatedText = draft.TranslatedText,
            SourceLanguage = draft.SourceLanguage,
            TargetLanguage = draft.TargetLanguage,
            Style = draft.Style,
            OperationType = draft.OperationType,
            IsFavorite = false,
            CreatedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
