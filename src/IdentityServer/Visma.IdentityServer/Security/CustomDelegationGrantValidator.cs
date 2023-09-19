using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using System.IdentityModel.Tokens.Jwt;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.DomainServices;
using Visma.IdentityServer.Extensions;

namespace Visma.IdentityServer.Security;

/// <summary>
/// Implements the <see cref="IExtensionGrantValidator"/>
/// </summary>
internal sealed class CustomDelegationGrantValidator : IExtensionGrantValidator
{
    private readonly IPersonService _personService;
    private readonly ITokenValidator _tokenValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDelegationGrantValidator"/>.
    /// </summary>
    public CustomDelegationGrantValidator(ITokenValidator validator,
                                          IPersonService personService)
    {
        _tokenValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    public string GrantType => "delegation";

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        var userToken = context.Request.Raw.Get("token");
        var scope = context.Request.Raw.Get("scope");

        if (string.IsNullOrEmpty(userToken) ||
            string.IsNullOrEmpty(scope))
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            return;
        }

        var result = await _tokenValidator.ValidateAccessTokenAsync(userToken);

        if (result.IsError)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            return;
        }

        JwtSecurityToken securityToken = userToken
                                            .Replace("Bearer ", string.Empty)
                                            .AsJwtSecurityToken();

        Person? person = await _personService.GetPersonByIdAsync(Guid.Parse(securityToken.Subject), CancellationToken.None);

        if(person is null)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidTarget);
            return;
        }

        bool reportingScope = IsReportingScope(scope);

        if (!reportingScope)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidScope);
            return;
        }
        
        if (!person.CanViewAccountingRecords)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);
            return;
        }

        _ = context.Request.RequestedScopes is null ? new List<string>() { scope } : context.Request.RequestedScopes.Append(scope);

        context.Result = new GrantValidationResult(securityToken.Subject, GrantType);
    }


    private bool IsReportingScope(string scope)
        => scope.Equals("reporting");
}
