using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace LinguaAI.Api.RateLimiting;

public static class TranslationRateLimiting
{
    public const string PolicyName = "translations";

    public static IServiceCollection AddTranslationRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var permitLimit = configuration.GetValue("RateLimiting:TranslationsPermitLimit", 20);
        var windowSeconds = configuration.GetValue("RateLimiting:TranslationsWindowSeconds", 60);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                var problemDetailsService = context.HttpContext.RequestServices
                    .GetService<IProblemDetailsService>();

                if (problemDetailsService is null)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync(
                        "Too many translation requests. Please try again later.",
                        cancellationToken);
                    return;
                }

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails =
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too Many Requests",
                        Detail = "Too many translation requests. Please try again later.",
                        Instance = context.HttpContext.Request.Path
                    }
                });
            };

            options.AddPolicy(PolicyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = Math.Clamp(permitLimit, 1, 200),
                        Window = TimeSpan.FromSeconds(Math.Clamp(windowSeconds, 10, 3600)),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        return services;
    }
}
