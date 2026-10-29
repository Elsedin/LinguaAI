using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaAI.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services (database, Identity, AI providers).
    /// Concrete registrations will be added in later phases.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Connection string is read here later when EF Core is configured.
        _ = configuration.GetConnectionString("DefaultConnection");

        return services;
    }
}
