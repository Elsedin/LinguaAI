using System.ComponentModel.DataAnnotations;

namespace LinguaAI.Application.Preferences;

public sealed class PreferenceDto
{
    public string? PreferredSourceLanguage { get; init; }
    public string? PreferredTargetLanguage { get; init; }
    public string? PreferredStyle { get; init; }
}

public sealed class UpdatePreferenceRequest
{
    [MaxLength(16)]
    public string? PreferredSourceLanguage { get; set; }

    [MaxLength(16)]
    public string? PreferredTargetLanguage { get; set; }

    [MaxLength(32)]
    public string? PreferredStyle { get; set; }
}

public interface IPreferenceService
{
    Task<PreferenceDto> GetAsync(string userId, CancellationToken cancellationToken = default);

    Task<PreferenceDto> UpsertAsync(
        string userId,
        UpdatePreferenceRequest request,
        CancellationToken cancellationToken = default);
}
