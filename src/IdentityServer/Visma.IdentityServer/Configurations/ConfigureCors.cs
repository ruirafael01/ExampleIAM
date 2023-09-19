using Duende.IdentityServer.Services;
using System.Diagnostics.CodeAnalysis;
using Visma.IdentityServer.Security;

namespace Visma.IdentityServer.Configurations;

/// <summary>
/// Configure CORS policies.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureCors
{
    /// <summary>
    /// Configure CORS default policy.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder ConfigureCorsPolicy(this WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true)
               .AllowCredentials()
               .Build();
            });
        });

        return appBuilder;
    }

    /// <summary>
    /// Configure CORS policy for Identity Server endpoints.
    /// </summary>
    /// <param name="appBuilder">The web application builder.</param>
    /// <returns>The web application builder.</returns>
    public static WebApplicationBuilder ConfigureIdentityServerCors(this WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddSingleton<ICorsPolicyService, CustomCorsPolicyService>();

        return appBuilder;
    }
}