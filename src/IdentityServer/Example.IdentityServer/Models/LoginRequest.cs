namespace Example.IdentityServer.Models;

/// <summary>
/// The model for a login request.
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// The email.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// The return url.
    /// </summary>
    public string ReturnUrl { get; init; } = string.Empty;
}
