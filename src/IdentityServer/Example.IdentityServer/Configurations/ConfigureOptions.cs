using Example.IdentityServer.Models.Options;
using System.Diagnostics.CodeAnalysis;

namespace Example.IdentityServer.Configurations;

/// <summary>
/// Configure all of the known application options.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ConfigureOptions
{
    /// <summary>
    /// Add all of the available runtime options.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static WebApplicationBuilder AddApplicationOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<DatabaseOptions>()
           .Bind(builder.Configuration.GetSection(nameof(DatabaseOptions)));

        builder.Services.AddOptions<ClientOptions>()
          .Bind(builder.Configuration.GetSection(nameof(ClientOptions)));

        builder.Services.AddOptions<IdentityServerOptions>()
          .Bind(builder.Configuration.GetSection(nameof(IdentityServerOptions)));

        builder.Services.AddOptions<ExternalSystemsOptions>()
        .Bind(builder.Configuration.GetSection(nameof(ExternalSystemsOptions)));

        return builder;
    }
}
