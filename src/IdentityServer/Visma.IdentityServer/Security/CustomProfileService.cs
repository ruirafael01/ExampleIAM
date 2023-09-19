using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Visma.IdentityServer.Domain.AggregatesModel.PersonAggregate;
using Visma.IdentityServer.Domain.DomainServices;
using Visma.IdentityServer.Extensions;

namespace Visma.IdentityServer.Security;

/// <summary>
/// Custom implementation of the <see cref="IProfileService"/>
/// </summary>
internal sealed class CustomProfileService : IProfileService
{
    private readonly ILogger<CustomProfileService> _logger;
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomProfileService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="claimsProvider">The claims provider.</param>
    public CustomProfileService(ILogger<CustomProfileService> logger, IPersonService personService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    /// <inheritdoc />
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        var subjectId = context.Subject.GetSubjectId();

        Person? person = await _personService.GetPersonByIdAsync(Guid.Parse(subjectId), CancellationToken.None);

        if (person is null)
        {
            _logger.LogDebug("Person {Person} not found in domain. Possible concurrency issue.", person);

            throw new InvalidOperationException("Cannot issue claims for a null person.");
        }
            
        context.IssuedClaims = person.AsClaims().ToList();
    }

    /// <inheritdoc />
    public async Task IsActiveAsync(IsActiveContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        var subjectId = context.Subject.GetSubjectId();

        Person? person = await _personService.GetPersonByIdAsync(Guid.Parse(subjectId), CancellationToken.None);

        _ = person is null ? context.IsActive = false : context.IsActive = true;
    }
}