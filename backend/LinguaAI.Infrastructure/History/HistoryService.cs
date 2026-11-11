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
        var filtered = ApplyFilters(db.TranslationHistories.Where(x => x.UserId == userId), query);
        var total = await filtered.CountAsync(cancellationToken);
        var items = await filtered
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

    public async Task<HistoryItemDto?> ToggleFavoriteAsync(
        string userId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.TranslationHistories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (row is null)
            return null;

        row.IsFavorite = !row.IsFavorite;
        await db.SaveChangesAsync(cancellationToken);

        return new HistoryItemDto
        {
            Id = row.Id,
            SourceText = row.SourceText,
            TranslatedText = row.TranslatedText,
            SourceLanguage = row.SourceLanguage,
            TargetLanguage = row.TargetLanguage,
            Style = row.Style,
            OperationType = row.OperationType,
            IsFavorite = row.IsFavorite,
            CreatedAtUtc = row.CreatedAtUtc
        };
    }

    public async Task<bool> DeleteAsync(
        string userId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.TranslationHistories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (row is null)
            return false;

        db.TranslationHistories.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<TranslationHistory> ApplyFilters(
        IQueryable<TranslationHistory> rows,
        HistoryQuery query)
    {
        var search = query.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
        {
            if (search.Length > 200)
                search = search[..200];

            var term = search.ToLowerInvariant();
            rows = rows.Where(x =>
                x.SourceText.ToLower().Contains(term) ||
                x.TranslatedText.ToLower().Contains(term));
        }

        var sourceLanguage = query.SourceLanguage?.Trim();
        if (!string.IsNullOrEmpty(sourceLanguage))
            rows = rows.Where(x => x.SourceLanguage == sourceLanguage);

        var targetLanguage = query.TargetLanguage?.Trim();
        if (!string.IsNullOrEmpty(targetLanguage))
            rows = rows.Where(x => x.TargetLanguage == targetLanguage);

        if (query.FavoritesOnly == true)
            rows = rows.Where(x => x.IsFavorite);

        return rows;
    }
}
