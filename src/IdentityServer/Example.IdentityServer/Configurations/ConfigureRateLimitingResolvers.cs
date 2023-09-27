using AspNetCoreRateLimit;
using Example.IdentityServer.Security;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Example.IdentityServer.Configuration;

/// <summary>
/// Configure custom rate limiting
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ConfigureRateLimitingResolvers : RateLimitConfiguration
{
    private readonly ILogger<EmailCustomResolver> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureRateLimitingResolvers"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="ipOptions">Ip Rate Limiting options</param>
    /// <param name="clientOptions">Client rate limiting options</param>
    public ConfigureRateLimitingResolvers(ILogger<EmailCustomResolver> logger, 
                                 IOptions<IpRateLimitOptions> ipOptions,
                                 IOptions<ClientRateLimitOptions> clientOptions) 
                                 : base(ipOptions, clientOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public override void RegisterResolvers()
    {
        base.RegisterResolvers();

        ClientResolvers.Add(new EmailCustomResolver(_logger));
    }
}
