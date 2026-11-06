using LinguaAI.Application.History;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<PagedResult<HistoryItemDto>> ListAsync(
        string userId,
        HistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var owned = db.TranslationHistories.Where(x => x.UserId == userId);
        var total = await owned.CountAsync(cancellationToken);
        var items = await owned
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new HistoryItemDto
            {
                Id = x.Id,
                SourceText = x.SourceText,
                TranslatedText = x.TranslatedText,
                SourceLanguage = x.SourceLanguage,
                TargetLanguage = x.TargetLanguage,
                Style = x.Style,
                OperationType = x.OperationType,
                IsFavorite = x.IsFavorite,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<HistoryItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
}
