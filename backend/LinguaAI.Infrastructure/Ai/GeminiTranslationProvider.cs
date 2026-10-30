using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinguaAI.Application.Abstractions;
using LinguaAI.Application.Translations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinguaAI.Infrastructure.Ai;

public sealed class GeminiTranslationProvider(
    HttpClient httpClient,
    IOptions<GeminiOptions> options,
    ILogger<GeminiTranslationProvider> logger) : ITranslationProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<TranslateResult> TranslateAsync(
        TranslateCommand command,
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Gemini API key is not configured.");

        var prompt = BuildPrompt(command);
        var path = $"models/{settings.Model}:generateContent";
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Headers.TryAddWithoutValidation("x-goog-api-key", settings.ApiKey);
        request.Content = JsonContent.Create(new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                responseMimeType = "application/json"
            }
        });

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Gemini returned {StatusCode}: {Body}", (int)response.StatusCode, Truncate(body));
            throw new HttpRequestException($"Translation provider returned {(int)response.StatusCode}.");
        }

        var payload = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("Translation provider returned an empty response.");

        var rawText = payload.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        if (string.IsNullOrWhiteSpace(rawText))
            throw new InvalidOperationException("Translation provider returned no text.");

        var parsed = JsonSerializer.Deserialize<GeminiTranslationPayload>(rawText, JsonOptions)
            ?? throw new InvalidOperationException("Translation provider returned invalid JSON.");

        if (string.IsNullOrWhiteSpace(parsed.TranslatedText))
            throw new InvalidOperationException("Translation provider returned empty translated text.");

        var source = string.IsNullOrWhiteSpace(parsed.DetectedSourceLanguage)
            ? command.SourceLanguage
            : parsed.DetectedSourceLanguage.Trim().ToLowerInvariant();

        if (string.Equals(command.SourceLanguage, SupportedLanguages.Auto, StringComparison.OrdinalIgnoreCase)
            && !SupportedLanguages.Codes.Contains(source))
            source = SupportedLanguages.Auto;

        return new TranslateResult(
            parsed.TranslatedText.Trim(),
            source,
            command.TargetLanguage,
            command.Style);
    }

    private static string BuildPrompt(TranslateCommand command)
    {
        var sourceLabel = string.Equals(command.SourceLanguage, SupportedLanguages.Auto, StringComparison.OrdinalIgnoreCase)
            ? "auto-detect"
            : command.SourceLanguage;

        var sb = new StringBuilder();
        sb.AppendLine("You are a translation engine. Respond with JSON only.");
        sb.AppendLine("Schema: {\"translatedText\":\"string\",\"detectedSourceLanguage\":\"bs|hr|sr|en|de|fr|es|it\"}");
        sb.AppendLine($"Source language: {sourceLabel}");
        sb.AppendLine($"Target language: {command.TargetLanguage}");
        sb.AppendLine($"Style: {command.Style}");
        sb.AppendLine("Translate the following text. Preserve meaning. Do not add commentary.");
        sb.AppendLine("---");
        sb.Append(command.Text);
        return sb.ToString();
    }

    private static string Truncate(string value) =>
        value.Length <= 300 ? value : value[..300];

    private sealed class GeminiGenerateContentResponse
    {
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private sealed class GeminiCandidate
    {
        public GeminiContent? Content { get; set; }
    }

    private sealed class GeminiContent
    {
        public List<GeminiPart>? Parts { get; set; }
    }

    private sealed class GeminiPart
    {
        public string? Text { get; set; }
    }

    private sealed class GeminiTranslationPayload
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }

        [JsonPropertyName("detectedSourceLanguage")]
        public string? DetectedSourceLanguage { get; set; }
    }
}
