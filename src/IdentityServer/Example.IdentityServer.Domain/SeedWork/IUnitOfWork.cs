using Example.IdentityServer.Domain.Models;

namespace Example.IdentityServer.Domain.SeedWork;

/// <summary>
/// The unit of work base interface.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves the current entities.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The domain response.</returns>
    Task<DomainResponse> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}