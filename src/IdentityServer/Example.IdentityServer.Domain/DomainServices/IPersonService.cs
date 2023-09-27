using Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

namespace Example.IdentityServer.Domain.DomainServices;

/// <summary>
/// Domain service for <see cref="Person"/>
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Gets a <see cref="Person"/> based on the e-mail.
    /// </summary>
    /// <param name="email">The e-mail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="Person"/> if it exists and null otherwise.</returns>
    Task<Person?> GetPersonByEmailAsync(PersonEmail email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a <see cref="Person"/> based on the identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="Person"/> if it exists and null otherwise.</returns>
    Task<Person?> GetPersonByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a <see cref="Person"/>.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="email">The e-mail.</param>
    /// <param name="language">The language.</param>
    /// <param name="role">The role-</param>
    /// <param name="name">The name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the <see cref="Person"/> is created on the domain.</returns>
    Task CreatePersonAsync(string password,
                           PersonEmail email,
                           PersonLanguage language,
                           PersonRole role,
                           PersonName name,
                           CancellationToken cancellationToken);

    /// <summary>
    /// Identifies if the input passwords is equal to the password of the person.
    /// </summary>
    /// <param name="person">The person.</param>
    /// <param name="inputPassword">The input password to be used for comparison.</param>
    /// <returns>True if passwords are equal and false if not.</returns>
    bool PasswordIsEqual(Person person, string inputPassword);
}
