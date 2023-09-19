using Visma.IdentityServer.Models;

namespace Visma.IdentityServer.Application.Services.Authentication;

/// <summary>
/// The service for authenticating users.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate a user based on the login request.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="httpContext">The current http context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The login response.</returns>
    Task<LoginResponse> AuthenticateAsync(LoginRequest request, HttpContext httpContext, CancellationToken cancellationToken);
}