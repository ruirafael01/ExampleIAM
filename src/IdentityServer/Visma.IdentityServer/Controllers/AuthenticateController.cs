using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Visma.IdentityServer.Models;
using IAuthenticationService = Visma.IdentityServer.Application.Services.Authentication.IAuthenticationService;

namespace Visma.IdentityServer.Controllers;

/// <summary>
/// Controller for authentication actions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthenticateController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateController"/>
    /// </summary>
    public AuthenticateController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    /// <summary>
    /// Login a user.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The login result.</returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authenticationService.AuthenticateAsync(request, HttpContext, cancellationToken);

        if (!string.IsNullOrEmpty(response.ErrorCode))
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>
    /// Login with external providers.
    /// </summary>
    /// <param name="provider">The provider for the external login.</param>
    /// <param name="returnUrl">The return URL.</param>
    /// <returns>A challenge with the external provider.</returns>
    [HttpGet("ExternalLogin")]
    public IActionResult ExternalLogin(string provider, string returnUrl)
        => throw new NotImplementedException();

    /// <summary>
    /// The callback for when the external provider challenge succeeds.
    /// </summary>
    /// <returns>The external login result.</returns>
    [HttpGet("ExternalLoginCallback")]
    public Task<IActionResult> ExternalLoginCallback()
        => throw new NotImplementedException();
}
