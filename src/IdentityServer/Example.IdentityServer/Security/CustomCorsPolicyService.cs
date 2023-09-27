using Duende.IdentityServer.Services;
using Example.IdentityServer.Models.Options;
using Microsoft.Extensions.Options;

namespace Example.IdentityServer.Security;

/// <summary>
/// Custom CORS policy to only allow configured clients AllowedCorsOrigins.
/// </summary>
internal sealed class CustomCorsPolicyService : DefaultCorsPolicyService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCorsPolicyService"/> class.
    /// </summary>
    public CustomCorsPolicyService(ILogger<CustomCorsPolicyService> logger,
                                   IOptions<ClientOptions> clientOptions) : base(logger)
    {
        _ = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = clientOptions ?? throw new ArgumentNullException(nameof(clientOptions));

        AllowedOrigins = clientOptions
                            .Value
                            .Clients
                            .Select(x => x.AllowedCorsOrigins)
                            .SelectMany(x => x)
                            .ToList();
    }
}
