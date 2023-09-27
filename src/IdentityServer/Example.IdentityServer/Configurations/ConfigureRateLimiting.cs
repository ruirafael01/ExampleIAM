using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Example.IdentityServer.Configuration;
using Example.IdentityServer.Models.Options;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace Example.IdentityServer.Configurations;

/// <summary>
/// Configure rate limiting.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureRateLimiting
{
    /// <summary>
    /// Configure CORS default policy.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder appBuilder)
    {
        ExternalSystemsOptions? configuration = appBuilder
                                                .Configuration
                                                .GetSection("ExternalSystemsOptions")
                                                .Get<ExternalSystemsOptions>();

        if (configuration is null)
            throw new InvalidOperationException("Cannot configure rate limiting with null external systems options.");

        var redisOptions = ConfigurationOptions.Parse(configuration.RedisConnectionString);

        appBuilder.Services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(redisOptions));

        appBuilder.Services.Configure<ClientRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.GeneralRules = new List<RateLimitRule>
            {
                  new RateLimitRule
                  {
                    Endpoint = "POST:/api/authenticate",
                      Period = "1m",
                      Limit = 3,
                  }
            };
        });

        appBuilder.Services.AddRedisRateLimiting();

        appBuilder.Services.AddSingleton<IRateLimitConfiguration, ConfigureRateLimitingResolvers>();

        return appBuilder;
    }
}