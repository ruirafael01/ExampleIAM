namespace Visma.IdentityServer.Models;

/// <summary>
/// Common constant variable declaration
/// </summary>
public static class Declares
{
    /// <summary>
    /// The route path for the authorize endpoint.
    /// </summary>
    public const string RoutePathAuthorize = "connect/authorize";

    /// <summary>
    /// The route path for the callback authorize endpoint.
    /// </summary>
    public const string RoutePathAuthorizeCallback = "connect/authorize/callback";

    /// <summary>
    /// The code for no context found.
    /// </summary>
    public const string NoContextFound = "NoContextFound";

    /// <summary>
    /// The code for a failed login.
    /// </summary>
    public const string LoginFailed = "FAIL";

    /// <summary>
    /// The code for a successful login.
    /// </summary>
    public const string LoginSuccess = "SUCCESS";

    /// <summary>
    /// The code for incorrect username or password.
    /// </summary>
    public const string IncorrectUsernameOrPassword = "IncorrectUsernameOrPassword";
}