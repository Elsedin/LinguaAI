using LinguaAI.Application.Preferences;
using LinguaAI.Application.Translations;
using LinguaAI.Infrastructure.Data;
using LinguaAI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Infrastructure.Preferences;

public sealed class PreferenceService(AppDbContext db) : IPreferenceService
{
    public async Task<PreferenceDto> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var row = await db.UserPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return row is null ? Empty() : ToDto(row);
    }

    public async Task<PreferenceDto> UpsertAsync(
        string userId,
        UpdatePreferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var source = NormalizeOptionalLanguage(request.PreferredSourceLanguage, allowAuto: true);
        var target = NormalizeOptionalLanguage(request.PreferredTargetLanguage, allowAuto: false);
        var style = NormalizeOptionalStyle(request.PreferredStyle);

        var row = await db.UserPreferences
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (row is null)
        {
            row = new UserPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };
            db.UserPreferences.Add(row);
        }

        row.PreferredSourceLanguage = source;
        row.PreferredTargetLanguage = target;
        row.PreferredStyle = style;

        await db.SaveChangesAsync(cancellationToken);
        return ToDto(row);
    }

    private static string? NormalizeOptionalLanguage(string? value, bool allowAuto)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return null;

        if (allowAuto && SupportedLanguages.IsValidSource(trimmed))
            return trimmed.ToLowerInvariant();

        if (!allowAuto && SupportedLanguages.IsValidTarget(trimmed))
            return trimmed.ToLowerInvariant();

        throw new ArgumentException(
            allowAuto ? "Preferred source language is not supported." : "Preferred target language is not supported.");
    }

    private static string? NormalizeOptionalStyle(string? value)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return null;

        if (!SupportedStyles.IsValid(trimmed))
            throw new ArgumentException("Preferred style is not supported.");

        return trimmed.ToLowerInvariant();
    }

    private static PreferenceDto Empty() => new();

    private static PreferenceDto ToDto(UserPreference row) => new()
    {
        PreferredSourceLanguage = row.PreferredSourceLanguage,
        PreferredTargetLanguage = row.PreferredTargetLanguage,
        PreferredStyle = row.PreferredStyle
    };
}
