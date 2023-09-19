using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.DomainServices;
using Visma.IdentityServer.Models;

namespace Visma.IdentityServer.Application.Services.Authentication;

/// <summary>
/// Implements the <see cref="IAuthenticationService"/>
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    private const string _invalidAuthentication = "Error authenticating user";
    private const string _invalidClientConfiguration = "Client configuration is not valid";
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IIdentityServerInteractionService _idsInteractionService;
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/>
    /// </summary>
    public AuthenticationService(ILogger<AuthenticationService> logger,
                                 IIdentityServerInteractionService idsInteractionService,
                                 IPersonService personService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _idsInteractionService = idsInteractionService ?? throw new ArgumentNullException(nameof(idsInteractionService));
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    /// <inheritdoc />
    public async Task<LoginResponse> AuthenticateAsync(LoginRequest request, HttpContext httpContext, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));
        _ = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

        if (!_idsInteractionService.IsValidReturnUrl(request.ReturnUrl))
            return FailedLogin(Declares.NoContextFound, _invalidClientConfiguration, Declares.LoginFailed);

        var context = await _idsInteractionService.GetAuthorizationContextAsync(request.ReturnUrl);

        if (context is null)
            return FailedLogin(Declares.NoContextFound, _invalidClientConfiguration, Declares.NoContextFound);

        Person? person = await _personService.GetPersonByEmailAsync(new PersonEmail(request.Email), cancellationToken);

        if (person is null)
            return FailedLogin(Declares.IncorrectUsernameOrPassword, _invalidAuthentication, _invalidAuthentication);

        bool passwordIsEqual = _personService.PasswordIsEqual(person, request.Password);

        if (!passwordIsEqual)
            return FailedLogin(Declares.IncorrectUsernameOrPassword, _invalidAuthentication, _invalidAuthentication);

        var user = new IdentityServerUser(person.Id.ToString())
        {
            DisplayName = person.Name.ToString()
        };

        await httpContext.SignInAsync(user);

        _logger.LogInformation("User {user} has been logged in.", user);

        return SuccessLogin(request.ReturnUrl);
    }

    private static LoginResponse FailedLogin(string error, string description, string code)
        => new LoginResponse()
        {
            Status = Declares.LoginFailed,
            Error = error,
            ErrorDescription = description,
            ErrorCode = code
        };

    private static LoginResponse SuccessLogin(string returnUrl)
        => new LoginResponse()
        {
            Status = Declares.LoginSuccess,
            RedirectUrl = returnUrl
        };
}