using System.Diagnostics.CodeAnalysis;
using Visma.IdentityServer.Models.Options;

namespace Visma.IdentityServer.Configurations;

/// <summary>
/// Configure IdentityServer health checks.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureHealthChecks
{
    /// <summary>
    /// Configure health checks.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder appBuilder)
    {
        ExternalSystemsOptions? externalSystemsOptions = appBuilder
                                               .Configuration
                                               .GetSection("ExternalSystemsOptions")
                                               .Get<ExternalSystemsOptions>();

        DatabaseOptions? databaseOptions = appBuilder
                                               .Configuration
                                               .GetSection("DatabaseOptions")
                                               .Get<DatabaseOptions>();

        if (externalSystemsOptions is null)
            throw new InvalidOperationException("Cannot configure health checks with null external systems options.");

        if (databaseOptions is null)
            throw new InvalidOperationException("Cannot configure health checks with null external database options.");

        appBuilder.Services.AddHealthChecks()
            .AddSqlServer(databaseOptions.ConnectionString)
            .AddRedis(externalSystemsOptions.RedisConnectionString);

        return appBuilder;
    }
}