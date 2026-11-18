using LinguaAI.Application.Dictionary;
using LinguaAI.Application.Translations;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Infrastructure.Dictionary;

public sealed class DictionaryService(AppDbContext db) : IDictionaryService
{
    public async Task<DictionaryEntryDto> CreateAsync(
        string userId,
        DictionaryEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        var values = Normalize(request);

        var entry = new DictionaryEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Term = values.Term,
            Translation = values.Translation,
            SourceLanguage = values.SourceLanguage,
            TargetLanguage = values.TargetLanguage,
            ExampleSentence = values.ExampleSentence,
            Notes = values.Notes,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.DictionaryEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(entry);
    }

    public async Task<PagedDictionaryResult> ListAsync(
        string userId,
        DictionaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var owned = db.DictionaryEntries.Where(x => x.UserId == userId);
        var total = await owned.CountAsync(cancellationToken);
        var items = await owned
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DictionaryEntryDto
            {
                Id = x.Id,
                Term = x.Term,
                Translation = x.Translation,
                SourceLanguage = x.SourceLanguage,
                TargetLanguage = x.TargetLanguage,
                ExampleSentence = x.ExampleSentence,
                Notes = x.Notes,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedDictionaryResult
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<DictionaryEntryDto?> UpdateAsync(
        string userId,
        Guid id,
        DictionaryEntryRequest request,
        CancellationToken cancellationToken = default)
    {
        var entry = await db.DictionaryEntries
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (entry is null)
            return null;

        var values = Normalize(request);
        entry.Term = values.Term;
        entry.Translation = values.Translation;
        entry.SourceLanguage = values.SourceLanguage;
        entry.TargetLanguage = values.TargetLanguage;
        entry.ExampleSentence = values.ExampleSentence;
        entry.Notes = values.Notes;

        await db.SaveChangesAsync(cancellationToken);
        return ToDto(entry);
    }

    public async Task<bool> DeleteAsync(
        string userId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entry = await db.DictionaryEntries
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (entry is null)
            return false;

        db.DictionaryEntries.Remove(entry);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static NormalizedEntry Normalize(DictionaryEntryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var term = request.Term?.Trim() ?? string.Empty;
        var translation = request.Translation?.Trim() ?? string.Empty;
        if (term.Length == 0)
            throw new ArgumentException("Term is required.");
        if (translation.Length == 0)
            throw new ArgumentException("Translation is required.");

        var source = request.SourceLanguage?.Trim() ?? string.Empty;
        var target = request.TargetLanguage?.Trim() ?? string.Empty;
        if (!SupportedLanguages.IsValidTarget(source))
            throw new ArgumentException("Source language is not supported.");
        if (!SupportedLanguages.IsValidTarget(target))
            throw new ArgumentException("Target language is not supported.");

        return new NormalizedEntry(
            term,
            translation,
            source.ToLowerInvariant(),
            target.ToLowerInvariant(),
            NormalizeOptional(request.ExampleSentence),
            NormalizeOptional(request.Notes));
    }

    private static string? NormalizeOptional(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static DictionaryEntryDto ToDto(DictionaryEntry entry) => new()
    {
        Id = entry.Id,
        Term = entry.Term,
        Translation = entry.Translation,
        SourceLanguage = entry.SourceLanguage,
        TargetLanguage = entry.TargetLanguage,
        ExampleSentence = entry.ExampleSentence,
        Notes = entry.Notes,
        CreatedAtUtc = entry.CreatedAtUtc
    };

    private sealed record NormalizedEntry(
        string Term,
        string Translation,
        string SourceLanguage,
        string TargetLanguage,
        string? ExampleSentence,
        string? Notes);
}
