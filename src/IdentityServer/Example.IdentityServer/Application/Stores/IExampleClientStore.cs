using Duende.IdentityServer.Models;

namespace Example.IdentityServer.Application.Stores;

/// <summary>
/// Store for <see cref="Client"/>
/// </summary>
internal interface IExampleClientStore
{
    /// <summary>
    /// List of clients.
    /// </summary>
    IReadOnlyList<Client> Clients { get; }

    /// <summary>
    /// Finds a client based on it's ID.
    /// </summary>
    /// <param name="clientId">The client ID.</param>
    /// <returns>The client if it exists and null if not.</returns>
    Client? FindByClientId(string clientId);
}