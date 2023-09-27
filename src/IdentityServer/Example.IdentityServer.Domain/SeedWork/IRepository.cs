namespace Example.IdentityServer.Domain.SeedWork;

/// <summary>
/// IRepository base interface.
/// </summary>
/// <typeparam name="T">Type of repository</typeparam>
public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}