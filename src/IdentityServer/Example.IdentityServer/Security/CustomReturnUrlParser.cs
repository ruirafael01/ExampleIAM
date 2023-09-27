using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Example.IdentityServer.Extensions;
using Example.IdentityServer.Models;
using Microsoft.Extensions.Options;
using IdentityServerOptions = Example.IdentityServer.Models.Options.IdentityServerOptions;

namespace Example.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="IReturnUrlParser"/>
/// </summary>
internal sealed class CustomReturnUrlParser : IReturnUrlParser
{
    private readonly string _identityServerURL;
    private readonly IAuthorizeRequestValidator _validator;
    private readonly IUserSession _userSession;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomReturnUrlParser"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="validator"></param>
    /// <param name="userSession"></param>
    /// <param name="identityServerOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public CustomReturnUrlParser(ILogger<CustomReturnUrlParser> logger,
                           IAuthorizeRequestValidator validator,
                           IUserSession userSession,
                           IOptions<IdentityServerOptions> identityServerOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _ = identityServerOptions ?? throw new ArgumentNullException(nameof(identityServerOptions));

        if (string.IsNullOrEmpty(identityServerOptions.Value.IssuerUri))
            throw new InvalidOperationException("Cannot perform return URL parsingwith null IssuerUri.");

        _identityServerURL = identityServerOptions.Value.IssuerUri.ToUpperInvariant();
    }

    /// <inheritdoc />
    public async Task<AuthorizationRequest> ParseAsync(string returnUrl)
    {
        if (!IsValidReturnUrl(returnUrl))
            return new AuthorizationRequest();

        var parameters = returnUrl.ReadQueryStringAsNameValueCollection();

        var user = await _userSession.GetUserAsync();

        var result = await _validator.ValidateAsync(parameters, user);

        if (result.IsError)
            return new AuthorizationRequest();

        _logger.LogTrace("AuthorizationRequest being returned");

        return ToAuthorizationRequest(result.ValidatedRequest);
    }

    /// <inheritdoc />
    public bool IsValidReturnUrl(string returnUrl)
    {
        var invariantURL = returnUrl.ToUpperInvariant();

        if (returnUrl.IsLocalUrl() || invariantURL.StartsWith(_identityServerURL))
        {
            var index = returnUrl.IndexOf('?');
            if (index >= 0)
            {
                returnUrl = returnUrl.Substring(0, index);
            }

            if (returnUrl.EndsWith(Declares.RoutePathAuthorize, StringComparison.Ordinal) ||
                returnUrl.EndsWith(Declares.RoutePathAuthorizeCallback, StringComparison.Ordinal))
            {
                _logger.LogTrace("ReturnUrl {0} is valid", returnUrl);

                return true;
            }
        }

        _logger.LogTrace("ReturnUrl {0} is not valid", returnUrl);

        return false;
    }

    private AuthorizationRequest ToAuthorizationRequest(ValidatedAuthorizeRequest request)
    {
        var authRequest = new AuthorizationRequest
        {
            Client = request.Client,
            RedirectUri = request.RedirectUri,
            DisplayMode = request.DisplayMode,
            UiLocales = request.UiLocales,
            IdP = request.GetIdP(),
            Tenant = request.GetTenant(),
            LoginHint = request.LoginHint
        };

        authRequest.Parameters.Add(request.Raw);

        return authRequest;
    }
}