using Duende.IdentityServer.Models;

namespace Example.IdentityServer.Models;

/// <summary>
/// In-memory configuration values.
/// </summary>
public static class Config
{
    /// <summary>
    /// Gets the available identity resources.
    /// </summary>
    /// <returns>A list containing the available identity resources.</returns>
    public static IEnumerable<IdentityResource> GetIdentityResources()
        => new IdentityResource[]
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
        };

    /// <summary>
    /// Gets the available api resources.
    /// </summary>
    /// <returns>A list containing the available api resources.</returns>
    public static IEnumerable<ApiResource> GetApis()
        => new ApiResource[]
        {
            new ApiResource("api", "Acme Fireworks Co. payroll")
        };

    /// <summary>
    /// Gets the available api scopes.
    /// </summary>
    /// <returns>A list containing the available api scopes.</returns>
    public static IEnumerable<ApiScope> GetApiScopes()
        => new ApiScope[]
        {
            new ApiScope("economic"),
            new ApiScope("accounting"),
            new ApiScope("reporting")
        };
};

