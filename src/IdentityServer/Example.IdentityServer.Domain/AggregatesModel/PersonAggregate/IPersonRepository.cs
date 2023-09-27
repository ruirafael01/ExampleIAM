using Example.IdentityServer.Domain.SeedWork;

namespace Example.IdentityServer.Domain.AggregatesModel.PersonAggregate;

/// <summary>
/// Person repository.
/// </summary>
public interface IPersonRepository : IRepository<Person>
{
    /// <summary>
    /// Get a specific person by e-mail.
    /// </summary>
    /// <param name="id">The e-mail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The person if it was found or null otherwise.</returns>
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Get a specific person.
    /// </summary>
    /// <param name="id">The person identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The person if it was found or null otherwise.</returns>
    Task<Person?> GetAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all of the persons.
    /// </summary>
    /// <returns>A list of all the persons if there are any persons or an empty list if there are none.</returns>
    IQueryable<Person> AllPersons { get; }

    /// <summary>
    /// Update a person.
    /// </summary>
    /// <param name="person">The person.</param>
    /// <param name="rowVersion">The version of the row.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that is completed when the person is updated.</returns>
    Task UpdateAsync(Person person, byte[] rowVersion, CancellationToken cancellationToken);

    /// <summary>
    /// Delete a person.
    /// </summary>
    /// <param name="id">The person identifier.</param>
    /// <param name="rowVersion">The version of the row.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that is completed when the person is deleted.</returns>
    Task DeleteAsync(Guid id, byte[] rowVersion, CancellationToken cancellationToken);

    /// <summary>
    /// Add a person.
    /// </summary>
    /// <param name="person">The person.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An instance of <see cref="Person"/>.</returns>
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken);
}