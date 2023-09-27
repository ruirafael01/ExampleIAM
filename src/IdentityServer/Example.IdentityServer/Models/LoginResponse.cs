namespace Example.IdentityServer.Models;

/// <summary>
/// Login result for authentication.
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// The status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The redirect url.
    /// </summary>
    public string RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// The Error if applicable.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// The Error description if applicable.
    /// </summary>
    public string ErrorDescription { get; set; } = string.Empty;

    /// <summary>
    /// The Error code if applicable.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;
}

