namespace Visma.IdentityServer.Models.Options;

/// <summary>
/// The identity server options model.
/// </summary>
internal sealed record IdentityServerOptions
{
    /// <summary>
    /// The login url.
    /// </summary>
    public string LoginUrl { get; init; } = string.Empty;

    /// <summary>
    /// The logout url.
    /// </summary>
    public string LogoutUrl { get; init; } = string.Empty;

    /// <summary>
    /// The error url.
    /// </summary>
    public string ErrorUrl { get; init; } = string.Empty;

    /// <summary>
    /// The issuer uri.
    /// </summary>
    public string IssuerUri { get; init; } = string.Empty;
}